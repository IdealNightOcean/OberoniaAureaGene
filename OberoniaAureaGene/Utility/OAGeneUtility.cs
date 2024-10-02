using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public static class OAGeneUtility
{
    public static MapComponent_OberoniaAureaGene GetOAGeneMapComp(this Map map)
    {
        return map.GetComponent<MapComponent_OberoniaAureaGene>();
    }
    public static float ComfyTemperatureMin(Pawn pawn) //Pawn的最低舒适温度
    {
        return pawn.GetStatValue(StatDefOf.ComfyTemperatureMin, applyPostProcess: true, 1);
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
}