using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public static class OAGeneUtility
{
    public static MapComponent_OberoniaAureaGene OAGeneMapComp(this Map map)
    {
        return map?.GetComponent<MapComponent_OberoniaAureaGene>();
    }
    public static MapComponent_LongSnowstorm LongSnowstormMapComp(this Map map)
    {
        return map?.GetComponent<MapComponent_LongSnowstorm>();
    }
    public static float ComfyTemperatureMin(Pawn pawn) //Pawn的最低舒适温度
    {
        return pawn.GetStatValue(StatDefOf.ComfyTemperatureMin, applyPostProcess: true);
    }
    public static bool IsSnowExtremeWeather(Map map) //是否为极端暴风雪（包括冰晶暴风雪）天气
    {
        if (map == null || map.weatherManager.curWeather == null)
        {
            return false;
        }
        return map.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_SnowExtreme || map.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_IceSnowExtreme;
    }
    public static void TryGiveEndSnowstormThought(Map map)
    {
        List<Pawn> pawns = map.mapPawns.AllHumanlikeSpawned;
        foreach (Pawn pawn in pawns)
        {
            if (pawn.IsMutant)
            {
                continue;
            }
            if (pawn.needs.mood?.thoughts.memories != null)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(OAGene_MiscDefOf.OAGene_Thought_SnowstormEnd);
            }
        }
    }

    //破坏风力发电机 (allMaps)
    public static void TryBreakPowerPlantWind(Map map, int duration)
    {
        BreakdownManager breakdownManager = map.GetComponent<BreakdownManager>();
        if (breakdownManager == null)
        {
            return;
        }
        List<CompBreakdownable> breakdownableComps = OAFrame_ReflectionUtility.GetFieldValue<List<CompBreakdownable>>(breakdownManager, "comps", null);
        if (breakdownableComps == null)
        {
            return;
        }
        CompPowerNormalPlantWind normalPlantWind;
        for (int i = 0; i < breakdownableComps.Count; i++)
        {
            normalPlantWind = breakdownableComps[i].parent.GetComp<CompPowerNormalPlantWind>();
            normalPlantWind?.Notify_ExtremeSnowstorm(duration);
        }
    }

    //范围内是否有支撑柱
    public static bool WithinRangeOfRoofHolder(IntVec3 c, Map map, float range)
    {
        bool connected = false;
        map.floodFiller.FloodFill(c, (IntVec3 x) => x.Roofed(map) && x.InHorDistOf(c, range), delegate (IntVec3 x)
        {
            for (int i = 0; i < 5; i++)
            {
                IntVec3 c2 = x + GenAdj.CardinalDirectionsAndInside[i];
                if (c2.InBounds(map) && c2.InHorDistOf(c, range))
                {
                    Building edifice = c2.GetEdifice(map);
                    if (edifice != null && edifice.def.holdsRoof)
                    {
                        if (edifice.def.building == null || !edifice.def.building.supportsWallAttachments)
                        {
                            connected = true;
                            return connected;
                        }
                    }
                }
            }
            return false;
        }, maxCellsToProcess: 500);
        return connected;
    }
}