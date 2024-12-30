using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormCultistArrival : IncidentWorker_IsolatedTraderCaravanArrival
{
    protected override IsolatedPawnGroupMakerDef PawnGroupMakerDef => Snowstorm_MiscDefOf.OAGene_GroupMaker_SnowstormCultist;

    protected override bool TryResolveFaction(IncidentParms parms)
    {
        parms.faction ??= OAFrame_FactionUtility.ValidTempFactionsOfDef(FactionDefOf.OutlanderCivil).Where(f => !f.HostileTo(Faction.OfPlayer)).RandomElementWithFallback(null);
        parms.faction ??= OAFrame_FactionUtility.GenerateTempFaction(FactionDefOf.OutlanderCivil);
        if (parms.faction != null)
        {
            parms.faction.factionHostileOnHarmByPlayer = true;
        }
        else
        {
            parms.faction = Find.FactionManager.RandomNonHostileFaction(allowNonHumanlike: false);
        }
        return parms.faction != null;
    }

    protected override List<Pawn> SpawnTradePawns(IncidentParms parms, PawnGroupMakerParms groupMakerParms, PawnGroupMaker groupMaker)
    {
        List<Pawn> pawns = base.SpawnTradePawns(parms, groupMakerParms, groupMaker);
        foreach (Pawn pawn in pawns)
        {
            if (pawn.RaceProps.Humanlike)
            {
                pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_SnowstormCultist);
            }
        }
        return pawns;
    }
    protected override void SendLetter(IncidentParms parms, List<Pawn> pawns)
    {
        SendStandardLetter(parms, pawns[0]);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        GameCondition snowstorm = map?.gameConditionManager.GetActiveCondition(OAGene_MiscDefOf.OAGene_ExtremeSnowstorm);
        if (snowstorm != null)
        {
            int delayTicks = snowstorm.TicksLeft + Rand.RangeInclusive(30000, 120000);
            OAFrame_MiscUtility.AddNewQueuedIncident(def, delayTicks, parms);
            return true;
        }
        return base.TryExecuteWorker(parms);
    }
}
