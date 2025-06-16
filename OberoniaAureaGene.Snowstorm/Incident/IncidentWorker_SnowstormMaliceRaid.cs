using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormMaliceRaid : IncidentWorker_RaidEnemy
{
    protected static readonly SimpleCurve MinPoints = [
        new CurvePoint(0,100),
        new CurvePoint(3,500),
        new CurvePoint(5,1000),
    ];

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
        if (parms.faction is null)
        {
            parms.faction = SnowstormUtility.RandomSnowstormMaliceRaidableFaction(map);
            if (parms.faction is null)
            {
                return false;
            }
        }
        float minPoints = MinPoints.Evaluate(GenDate.YearsPassed);
        parms.points = Mathf.Max(minPoints, parms.points);
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
        if (pawns is not null)
        {
            foreach (Pawn pawn in pawns)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    Snowstorm_MiscUtility.SetColdPreparation(pawn, Snowstorm_HediffDefOf.OAGene_Hediff_ColdPreparation_Enemy);
                }
            }
        }
    }
}
