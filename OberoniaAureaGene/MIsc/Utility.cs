using RimWorld;
using System;
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

[StaticConstructorOnStartup]
public static class FastEffectRandom
{
    private const double REAL_UNIT_INT = 4.656612873077393E-10;

    private const double REAL_UNIT_UINT = 2.3283064365386963E-10;

    private const uint Y = 842502087u;

    private const uint Z = 3579807591u;

    private const uint W = 273326509u;

    private static uint x;

    private static uint y;

    private static uint z;

    private static uint w;

    static FastEffectRandom()
    {
        Reinitialise(Environment.TickCount);
    }

    public static void Reinitialise(int seed)
    {
        x = (uint)seed;
        y = 842502087u;
        z = 3579807591u;
        w = 273326509u;
    }

    public static int Next(int lowerBound, int upperBound)
    {
        if (lowerBound > upperBound)
        {
            throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be equal to or large than lowerBound");
        }
        uint num = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        int num2 = upperBound - lowerBound;
        if (num2 < 0)
        {
            return lowerBound + (int)(2.3283064365386963E-10 * (double)(w = w ^ (w >> 19) ^ (num ^ (num >> 8))) * (double)((long)upperBound - (long)lowerBound));
        }
        return lowerBound + (int)(4.656612873077393E-10 * (double)(int)(0x7FFFFFFF & (w = w ^ (w >> 19) ^ (num ^ (num >> 8)))) * (double)num2);
    }
}