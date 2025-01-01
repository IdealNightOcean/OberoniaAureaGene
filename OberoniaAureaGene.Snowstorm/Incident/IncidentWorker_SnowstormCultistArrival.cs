using OberoniaAurea_Frame;
using OberoniaAurea_Frame.Utility;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

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
        GameCondition snowstorm = SnowstormUtility.SnowstormCondition;
        if (snowstorm != null)
        {
            if (snowstorm.Permanent)
            {
                return false;
            }
            else
            {
                int delayTicks = snowstorm.TicksLeft + Rand.RangeInclusive(30000, 120000);
                OAFrame_MiscUtility.AddNewQueuedIncident(def, delayTicks, parms);
                return true;
            }
        }
        if (!TryResolveParms(parms))
        {
            return false;
        }
        if (parms.faction.HostileTo(Faction.OfPlayer))
        {
            return false;
        }
        Map map = (Map)parms.target;
        PawnGroupMakerParms groupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDef, parms, ensureCanGenerateAtLeastOnePawn: true);
        groupMakerParms.tile = Tile.Invalid;
        if (!OAFrame_PawnGenerateUtility.TryGetRandomPawnGroupMaker(PawnGroupKindDef, PawnGroupMakerDef, out PawnGroupMaker groupMaker))
        {
            return false;
        }
        List<Pawn> pawns = SpawnTradePawns(parms, groupMakerParms, groupMaker);
        if (pawns.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i].needs != null && pawns[i].needs.food != null)
            {
                pawns[i].needs.food.CurLevel = pawns[i].needs.food.MaxLevel;
            }
        }
        SendLetter(parms, pawns);
        RCellFinder.TryFindRandomSpotJustOutsideColony(pawns[0].Position, pawns[0].MapHeld, pawns[0], out IntVec3 result, delegate (IntVec3 c)
        {
            for (int k = 0; k < pawns.Count; k++)
            {
                if (!pawns[k].CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                {
                    return false;
                }
            }
            return true;
        });
        LordJob_SnowstormCultistTradeWithColony lordJob = new(parms.faction, result);
        LordMaker.MakeNewLord(parms.faction, lordJob, map, pawns);
        return true;
    }
}
