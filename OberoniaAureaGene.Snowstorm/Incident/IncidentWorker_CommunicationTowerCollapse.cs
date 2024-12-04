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
        return SnowstormUtility.IsSnowExtremeWeather(map);
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!TryResolveParms(parms))
        {
            return false;
        }
        Map map = (Map)parms.target;
        GameCondition_ExtremeSnowstorm snowstorm = map.gameConditionManager.GetActiveCondition<GameCondition_EndGame_ExtremeSnowstorm>();
        if (snowstorm != null && !snowstorm.blockCommsconsole)
        {
            snowstorm.blockCommsconsole = true;
            SendStandardLetter(parms, null);
        }
        return true;
    }
}
