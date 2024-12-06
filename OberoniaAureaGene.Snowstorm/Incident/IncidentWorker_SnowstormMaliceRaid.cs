using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormMaliceRaid : IncidentWorker_RaidEnemy
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        return base.CanFireNowSub(parms);
    }
    public bool TryResolveParms(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        if (parms.faction == null)
        {
            parms.faction = SnowstormUtility.RandomSnowstormMaliceRaidableFaction(map);
            if (parms.faction == null)
            {
                return false;
            }
        }
        parms.points = Mathf.Max(1000, parms.points);
        return true;
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!TryResolveParms(parms))
        {
            return false;
        }
        IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, parms.target);
        raidParms.forced = true;
        raidParms.faction = parms.faction;
        raidParms.raidStrategy = Snowstorm_MiscDefOf.OAGene_SnowstormImmediateAttackBreaching;
        raidParms.points = Mathf.Max(raidParms.points, parms.points);
        return base.TryExecuteWorker(raidParms);
    }
    protected override void PostProcessSpawnedPawns(IncidentParms parms, List<Pawn> pawns)
    {
        if (pawns != null)
        {
            foreach (Pawn pawn in pawns)
            {
                pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_PreparationWarm);
            }
        }
    }
}
