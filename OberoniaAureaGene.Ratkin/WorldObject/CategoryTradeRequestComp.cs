using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class WorldObjectCompProperties_CategoryTradeRequestComp : WorldObjectCompProperties
{
    public WorldObjectCompProperties_CategoryTradeRequestComp()
    {
        compClass = typeof(CategoryTradeRequestComp);
    }
}

[StaticConstructorOnStartup]
public class CategoryTradeRequestComp : WorldObjectComp
{
    protected bool active;
    public ThingCategoryDef requestedCategoryDef;
    public int requestCount;
    public int remainingThingCount;
    public int requestQuality = -1;
    public bool isApparel;
    public int expiration = -1;
    public string outSignalFulfilled;
    private static readonly Texture2D TradeCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/FulfillTradeRequest");

    public bool ActiveRequest => active && expiration > Find.TickManager.TicksGame;

    public virtual void InitTradeRequest(ThingCategoryDef requestedCategoryDef, int requestCount, int expirationDelay, int requestQuality = -1, bool isApparel = false)
    {
        this.requestedCategoryDef = requestedCategoryDef;
        this.requestCount = requestCount;
        remainingThingCount = requestCount;
        this.requestQuality = requestQuality;
        this.isApparel = isApparel;
        expiration = Find.TickManager.TicksGame + expirationDelay;
        active = true;
    }

    public override string CompInspectStringExtra()
    {
        if (ActiveRequest)
        {
            return "OAGene_CaravanCategoryRequestInfo".Translate(RequestedThingCategoryLabel(requestedCategoryDef, requestCount, requestQuality, isApparel).CapitalizeFirst(), (expiration - Find.TickManager.TicksGame).ToStringTicksToDays());
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

    public void Disable()
    {
        active = false;
        expiration = -1;
        requestedCategoryDef = null;
        requestCount = 0;
        remainingThingCount = 0;
        requestQuality = -1;
        isApparel = false;
    }

    private Command FulfillRequestCommand(Caravan caravan)
    {
        Command_Action command_Action = new()
        {
            defaultLabel = "CommandFulfillTradeOffer".Translate(),
            defaultDesc = "CommandFulfillTradeOfferDesc".Translate(),
            icon = TradeCommandTex,
            action = delegate
            {
                if (!ActiveRequest)
                {
                    Log.Error("Attempted to fulfill an unavailable request");
                }
                else if (!OAGene_RatkinUtility.HasAnyThings(caravan, requestedCategoryDef, PlayerCanGive))
                {
                    Messages.Message("CommandFulfillTradeOfferFailInsufficient".Translate(RequestedThingCategoryLabel(requestedCategoryDef, requestCount, requestQuality, isApparel)), MessageTypeDefOf.RejectInput, historical: false);
                }
                else
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("OAGene_CommandFulfillCategoryTradeConfirm".Translate(requestedCategoryDef), delegate
                    {
                        Fulfill(caravan);
                    }));
                }
            }
        };
        if (!OAGene_RatkinUtility.HasAnyThings(caravan, requestedCategoryDef, PlayerCanGive))
        {
            command_Action.Disable("OAGene_CommandFulfillCategoryTradeFailInsufficient".Translate(RequestedThingCategoryLabel(requestedCategoryDef, 1, requestQuality, isApparel)));
        }
        return command_Action;
    }

    private void Fulfill(Caravan caravan)
    {
        List<Thing> list = CaravanInventoryUtility.TakeThings(caravan, delegate (Thing thing)
        {
            if (!thing.def.thingCategories.Contains(requestedCategoryDef))
            {
                return 0;
            }
            if (!PlayerCanGive(thing))
            {
                return 0;
            }
            int num = Mathf.Min(remainingThingCount, thing.stackCount);
            remainingThingCount -= num;
            return num;
        });
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Destroy();
        }
        if (remainingThingCount <= 0)
        {
            if (parent.Faction != null)
            {
                Faction.OfPlayer.TryAffectGoodwillWith(parent.Faction, 12, canSendMessage: true, canSendHostilityLetter: true, HistoryEventDefOf.QuestGoodwillReward);
            }
            QuestUtility.SendQuestTargetSignals(parent.questTags, "TradeRequestFulfilled", parent.Named("SUBJECT"), caravan.Named("CARAVAN"));
            Disable();
        }
        else
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("OAGene_FulfillCategoryTradePartComplete".Translate(requestedCategoryDef, requestCount - remainingThingCount, remainingThingCount), null));
        }
    }

    private bool PlayerCanGive(Thing thing)
    {
        if (thing.GetRotStage() != 0)
        {
            return false;
        }
        if (thing is Apparel apparel)
        {
            if (isApparel)
            {
                return !apparel.WornByCorpse;
            }
            else
            {
                return false;
            }
        }
        if (requestQuality >= 0)
        {
            CompQuality compQuality = thing.TryGetComp<CompQuality>();
            if (compQuality == null || (int)compQuality.Quality < requestQuality)
            {
                return false;
            }
        }
        return true;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Defs.Look(ref requestedCategoryDef, "requestedCategoryDef");
        Scribe_Values.Look(ref requestCount, "requestCount", 0);
        Scribe_Values.Look(ref remainingThingCount, "remainingThingCount", 0);
        Scribe_Values.Look(ref requestQuality, "requestQuality", -1);
        Scribe_Values.Look(ref expiration, "expiration", 0);
        Scribe_Values.Look(ref active, "active", defaultValue: false);
        BackCompatibility.PostExposeData(this);
    }

    public static string RequestedThingCategoryLabel(ThingCategoryDef def, int count, int needQuality, bool isApparel)
    {
        string text = "OAGene_RequestedThingCategoryLabel".Translate(def.label, count);
        if (needQuality>=0)
        {
            text += " (" + "NormalQualityOrBetter".Translate() + ")";
        }

        if (isApparel)
        {
            text += " (" + "NotTainted".Translate() + ")";
        }

        return text;
    }
}
