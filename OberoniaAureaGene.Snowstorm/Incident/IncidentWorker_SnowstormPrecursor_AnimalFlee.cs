using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene.Snowstorm;

internal class IncidentWorker_SnowstormPrecursor_AnimalFlee : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (map == null || !map.gameConditionManager.ConditionIsActive(Snowstrom_MiscDefOf.OAGene_SnowstormPrecursor))
        {
            return false;
        }
        return true;
    }

    public bool TryResolveParms(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (map == null)
        {
            map = Find.RandomPlayerHomeMap;
            if (map == null || !map.gameConditionManager.ConditionIsActive(Snowstrom_MiscDefOf.OAGene_SnowstormPrecursor))
            {
                return false;
            }
            parms.target = map;
        }
        return true;
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!TryResolveParms(parms))
        {
            return false;
        }
        Map map = (Map)parms.target;
        Faction ofPlayer = Faction.OfPlayer;
        IEnumerable<Pawn> animals = map.mapPawns.AllPawnsSpawned.Where(p => p.Faction != ofPlayer && p.RaceProps.Animal);
        foreach (Pawn animal in animals)
        {
            Job job = TryGiveExitJob(animal);
            if (job != null)
            {
                animal.jobs.TryTakeOrderedJob(job, JobTag.Escaping);
            }
        }
        return true;

    }

    protected static Job TryGiveExitJob(Pawn pawn)
    {
        if (pawn.Downed && !pawn.Crawling)
        {
            return null;
        }
        if (!RCellFinder.TryFindBestExitSpot(pawn, out IntVec3 dest, TraverseMode.ByPawn, canBash: true))
        {
            return null;
        }

        Job job = JobMaker.MakeJob(JobDefOf.Goto, dest);
        job.exitMapOnArrival = true;
        job.locomotionUrgency = PawnUtility.ResolveLocomotion(pawn, LocomotionUrgency.Sprint, LocomotionUrgency.Jog);
        job.expiryInterval = 20000;
        job.canBashDoors = true;
        return job;
    }
}
