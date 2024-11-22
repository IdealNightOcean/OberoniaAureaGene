using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_MakeGameCondition_IcaRain : IncidentWorker_MakeGameCondition
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        return base.CanFireNowSub(parms);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        return base.TryExecuteWorker(parms);
    }
}
