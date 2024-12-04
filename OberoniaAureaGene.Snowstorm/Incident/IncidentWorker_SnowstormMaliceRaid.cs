using OberoniaAurea_Frame;
using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormMaliceRaid : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        return SnowstormUtility.IsSnowExtremeWeather(map);
    }

    public bool TryResolveParms(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (map == null)
        {
            map = Find.RandomPlayerHomeMap;
            if (map == null || !SnowstormUtility.IsSnowExtremeWeather(map))
            {
                return false;
            }
            parms.target = map;
        }
        if (parms.faction == null)
        {
            parms.faction = SnowstormUtility.RandomSnowstromMaliceRaidableFaction(map);
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
        raidParms.raidStrategy = Snowstrom_MiscDefOf.OAGene_SnowstormImmediateAttackBreaching;
        raidParms.points = Mathf.Max(raidParms.points, parms.points);
        return OAFrame_MiscUtility.TryFireIncidentNow(IncidentDefOf.RaidEnemy, raidParms);
    }
}