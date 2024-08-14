using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class IncidentWorker_TravelRatkinTraderGroup : IncidentWorker_TraderCaravanArrival
{
    protected static readonly IntRange TraderCount = new(5, 6);
    protected static readonly IntRange DelayTicks = new(20000, 24000);

    protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
    {
        if (f.def != OAGene_RatkinDefOf.Rakinia_TravelRatkin)
        {
            return false;
        }
        return base.FactionCanBeGroupSource(f, map, desperate);
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!TryResolveParms(parms))
        {
            return false;
        }
        if (parms.faction.HostileTo(Faction.OfPlayer))
        {
            return false;
        }
        IncidentParms subParms = new()
        {
            target = map,
            faction = parms.faction,
            forced = true
        };
        subParms.faction.def.caravanTraderKinds.TryRandomElementByWeight((TraderKindDef traderDef) => TraderKindCommonality(traderDef, map, subParms.faction), out subParms.traderKind);
        IncidentDefOf.TraderCaravanArrival.Worker.TryExecute(subParms);
        int traderCount = TraderCount.RandomInRange - 1;
        int delayTicks = 0;
        for (int i = 0; i < traderCount; i++)
        {
            delayTicks += DelayTicks.RandomInRange;
            TraderCaravanArrival(map, parms.faction, delayTicks);
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
        Find.Storyteller.incidentQueue.Add(IncidentDefOf.TraderCaravanArrival, Find.TickManager.TicksGame + delayTicks, parms);
    }

    protected void SendLetter(IncidentParms parms)
    {
        TaggedString letterLabel = "OAGene_LetterLabelTravelRatkinTraderGroup".Translate(parms.faction.Name).CapitalizeFirst();
        TaggedString letterText = "OAGene_LetterTravelRatkinTraderGroup".Translate(parms.faction.NameColored).CapitalizeFirst();
        SendStandardLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, parms, null);
    }
}
