using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_SnowstormPrecursor : GameCondition_TemperatureChange
{

    public override void End()
    {
        Map mainMap = GetMainMap();
        if (mainMap != null)
        {
            IncidentParms parms = new()
            {
                target = mainMap,
            };
            Snowstrom_IncidentDefOf.OAGene_ExtremeSnowstorm.Worker.TryExecute(parms);
        }
        base.End();
    }

    private Map GetMainMap()
    {
        Map mainMap = gameConditionManager.ownerMap;

        mainMap ??= AffectedMaps.Where(m => m.IsPlayerHome).RandomElementWithFallback(null);

        mainMap ??= Find.AnyPlayerHomeMap;

        return mainMap;
    }
}
