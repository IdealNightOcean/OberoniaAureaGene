using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class WorldObjectCompProperties_SaleRequestComp : WorldObjectCompProperties
{
    public WorldObjectCompProperties_SaleRequestComp()
    {
        compClass = typeof(SaleRequestComp);
    }
}

public class SaleRequestComp : WorldObjectComp
{
    protected bool active;
    public ThingDef requestThingDef;
    public int requestCount;
    public int expiration = -1;
    public string outSignalGived;
    private static readonly Texture2D TradeCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/FulfillTradeRequest");

    public bool ActiveRequest => active && expiration > Find.TickManager.TicksGame;

    public override string CompInspectStringExtra()
    {
        if (ActiveRequest)
        {
            return "CaravanRequestInfo".Translate(TradeRequestUtility.RequestedThingLabel(requestThingDef, requestCount).CapitalizeFirst(), (expiration - Find.TickManager.TicksGame).ToStringTicksToDays(), (requestThingDef.GetStatValueAbstract(StatDefOf.MarketValue) * (float)requestCount).ToStringMoney());
        }
        return null;
    }

    public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
    {
        if (ActiveRequest && CaravanVisitUtility.SettlementVisitedNow(caravan) == parent)
        {
            yield return FulfillRequestCommand(caravan);
        }
    }
    public void InitSaleRequest(ThingDef thingDef, int thingCount, int expirationDelay)
    {
        requestThingDef = thingDef;
        requestCount = thingCount;
        expiration = Find.TickManager.TicksGame + expirationDelay;
        active = true;
    }
    public void Disable()
    {
        active = false;
        expiration = -1;
        requestThingDef = null;
        requestCount = 0;
    }

    private Command FulfillRequestCommand(Caravan caravan)
    {
        Command_Action command_Action = new()
        {
            defaultLabel = "OAGene_CommandReciveSaleOffer".Translate(),
            defaultDesc = "OAGene_CommandReciveSaleOfferDesc".Translate(),
            icon = TradeCommandTex,
            action = delegate
            {
                if (!ActiveRequest)
                {
                    Log.Error("Attempted to fulfill an unavailable request");
                }
                else
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("CommandFulfillTradeOfferConfirm".Translate(GenLabel.ThingLabel(requestThingDef, null, requestCount)), delegate
                    {
                        Fulfill(caravan);
                    }));
                }
            }
        };
        return command_Action;
    }
    private void Fulfill(Caravan caravan)
    {
        List<Thing> things = OberoniaAureaFrameUtility.TryGenerateThing(requestThingDef, requestCount);
        foreach (Thing t in things)
        {
            CaravanInventoryUtility.GiveThing(caravan, t);
        }
        if (parent.Faction != null)
        {
            Faction.OfPlayer.TryAffectGoodwillWith(parent.Faction, 12, canSendMessage: true, canSendHostilityLetter: true, HistoryEventDefOf.QuestGoodwillReward);
        }
        QuestUtility.SendQuestTargetSignals(parent.questTags, "OAGene_SaleRequestFulfilled", parent.Named("SUBJECT"), caravan.Named("CARAVAN"));
        Disable();
    }
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref active, "active", defaultValue: false);
        Scribe_Values.Look(ref expiration, "expiration", -1);
        Scribe_Defs.Look(ref requestThingDef, "requestThingDef");
        Scribe_Values.Look(ref requestCount, "requestCount", 0);
        BackCompatibility.PostExposeData(this);
    }
}
