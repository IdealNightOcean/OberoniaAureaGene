using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_EndGame_SnowstormPrecursor : GameCondition_TemperatureChange
{
    public override void Init()
    {
        base.Init();
        Map mainMap = GetMainMap();
        if (mainMap != null)
        {
            IncidentParms parms = new()
            {
                target = mainMap,
            };
            Snowstrom_IncidentDefOf.OAGene_SnowstormPrecursor_AnimalFlee.Worker.TryExecute(parms);
        }
    }

    public override void End()
    {
        base.End();

        GameCondition gameCondition = GameConditionMaker.MakeCondition(Snowstrom_MiscDefOf.OAGene_EndGame_ExtremeSnowstorm);
        gameConditionManager.RegisterCondition(gameCondition);
    }

    private Map GetMainMap()
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
