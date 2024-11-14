using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_CommunicationTowerCollapse : IncidentWorker
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
        return true;
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!TryResolveParms(parms))
        {
            return false;
        }
        Map map = (Map)parms.target;
        GameCondition_ExtremeSnowstorm snowstorm = (GameCondition_ExtremeSnowstorm)map.gameConditionManager.GetActiveCondition(OAGene_MiscDefOf.OAGene_ExtremeSnowstorm);
        if (snowstorm != null && !snowstorm.blockCommsconsole)
        {
            snowstorm.blockCommsconsole = true;
        }
        return true;
    }
}
