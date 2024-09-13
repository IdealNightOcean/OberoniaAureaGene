using RimWorld;
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
    public static bool IsSnowExtremeWeather(Map map) //是否为极端暴风雪天气
    {
        return map?.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_SnowExtreme;
    }
}