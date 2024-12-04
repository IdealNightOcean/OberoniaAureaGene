using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_EndGame_SnowstormPrecursor : GameCondition_SnowstormPrecursor
{
    protected override void PostEnd()
    {
        Map mainMap = GetMainMap();
        if (mainMap != null)
        {
            IncidentParms parms = new()
            {
                target = Find.World,
            };
            OAFrame_MiscUtility.TryFireIncidentNow(Snowstrom_IncidentDefOf.OAGene_ExtremeSnowstorm, parms);
        }
    }

    protected override Map GetMainMap()
    {
        Map mainMap = gameConditionManager.ownerMap;

        if (mainMap == null)
        {
            MapParent hometown = Find.WorldObjects.AllWorldObjects.Where(o => o.def == Snowstrom_MiscDefOf.OAGene_Hometown).FirstOrFallback() as MapParent;
            if (hometown != null)
            {
                mainMap = hometown.Map;
            }
        }

        return mainMap;
    }

}
