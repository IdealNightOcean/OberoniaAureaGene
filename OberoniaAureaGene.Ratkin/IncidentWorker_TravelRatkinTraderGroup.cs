using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class IncidentWorker_TravelRatkinTraderGroup : IncidentWorker_TraderCaravanArrival
{
    protected static readonly IntRange TraderCount = new(3, 4);
    protected static readonly IntRange DelayTicks = new(20000, 24000);

    public override bool FactionCanBeGroupSource(Faction f, IncidentParms parms, bool desperate = false)
    {
        if (f.def != OAGene_RatkinDefOf.Rakinia_TravelRatkin)
        {
            return false;
        }
        return base.FactionCanBeGroupSource(f, parms, desperate);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;

        if (!TryResolveParms(parms))
        {
            return false;
        }
        Faction faction = parms.faction;
        if (faction.HostileTo(Faction.OfPlayer))
        {
            return false;
        }
        IncidentParms subParms = new()
        {
            target = map,
            faction = faction,
            forced = true
        };
        faction.def.caravanTraderKinds.TryRandomElementByWeight((TraderKindDef traderDef) => TraderKindCommonality(traderDef, map, faction), out subParms.traderKind);
        OAFrame_MiscUtility.TryFireIncidentNow(IncidentDefOf.TraderCaravanArrival, subParms);
        int traderCount = TraderCount.RandomInRange;
        int delayTicks = 0;
        for (int i = 0; i < traderCount - 1; i++)
        {
            delayTicks += DelayTicks.RandomInRange;
            TraderCaravanArrival(map, faction, delayTicks);
        }

        SendLetter(parms);
        return true;
    }


    protected void TraderCaravanArrival(Map map, Faction faction, int delayTicks)
    {
        faction.def.caravanTraderKinds.TryRandomElementByWeight((TraderKindDef traderDef) => TraderKindCommonality(traderDef, map, faction), out TraderKindDef traderKind);
        IncidentParms parms = new()
        {
            target = map,
            faction = faction,
            traderKind = traderKind,
            forced = true
        };
        OAFrame_MiscUtility.AddNewQueuedIncident(IncidentDefOf.TraderCaravanArrival, delayTicks, parms);
    }

    protected void SendLetter(IncidentParms parms)
    {
        TaggedString letterLabel = "OAGene_LetterLabelTravelRatkinTraderGroup".Translate(parms.faction.Name).CapitalizeFirst();
        TaggedString letterText = "OAGene_LetterTravelRatkinTraderGroup".Translate(parms.faction.NameColored).CapitalizeFirst();
        SendStandardLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, parms, null);
    }
}
