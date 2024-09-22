using OberoniaAurea_Frame;
using RimWorld;
using System;
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

}

[StaticConstructorOnStartup]
public static class SnowstormUtility
{
    public static bool IsSnowExtremeWeather(Map map) //是否为极端暴风雪（包括冰晶暴风雪）天气
    {
        if (map == null)
        {
            return false;
        }
        return map.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_SnowExtreme || map.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_IceSnowExtreme;
    }
    public static bool IsIceStormWeather(Map map) //是否为冰晶暴风雪天气
    {
        return map?.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_IceSnowExtreme;
    }
    public static void InitExtremeSnowstorm(Map map, int duration)
    {
        if (map == null)
        {
            return;
        }
        map.weatherManager.TransitionTo(OAGene_MiscDefOf.OAGene_SnowExtreme);
        map.GetOAGeneMapComp()?.Notify_Snow(duration);
        TryBreakPowerPlantWind(map, duration);
        if (map.IsPlayerHome)
        {
            TryInitSnowstormRaid(map);
        }
    }
    public static void EndExtremeSnowstorm(Map map)
    {
        if (map == null)
        {
            return;
        }
        map.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowGentle);
        TryGiveEndSnowstormThought(map);
    }
    public static void TryBreakPowerPlantWind(Map map, int duration) //破坏风力发电机
    {
        BreakdownManager breakdownManager = map.GetComponent<BreakdownManager>();
        if (breakdownManager == null)
        {
            return;
        }
        List<CompBreakdownable> breakdownableComps = ReflectionUtility.GetFieldValue<List<CompBreakdownable>>(breakdownManager, "comps", null);
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
    public static void TryInitSnowstormRaid(Map map) //暴风雪破墙袭击
    {
        if (GenDate.DaysPassed < 60)
        {
            return;
        }
        if (Rand.Bool)
        {
            Faction faction = Find.FactionManager.RandomRaidableEnemyFaction(allowNonHumanlike: false);
            if (faction == null)
            {

            }
            if (faction == null)
            {
                return;
            }
            IncidentParms incidentParms = new()
            {
                target = map,
                faction = faction,
                points = StorytellerUtility.DefaultThreatPointsNow(map),
                raidStrategy = OAGene_MiscDefOf.OAGene_SnowstormImmediateAttackBreaching,
            };
            int raidCount = Rand.Bool ? 1 : 2;
            int delayTicks = new IntRange().RandomInRange;
            for (int i = 0; i < raidCount; i++)
            {
                delayTicks += new IntRange().RandomInRange;
                Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + delayTicks, incidentParms);
            }
        }
    }
    public static void TryGiveEndSnowstormThought(Map map)
    {
        List<Pawn> pawns = map.mapPawns.AllHumanlikeSpawned;
        foreach (Pawn pawn in pawns)
        {
            if (pawn.needs.mood?.thoughts.memories != null)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(OAGene_MiscDefOf.OAGene_Thought_SnowstormEnd);
            }
            if (pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                pawn.health.AddHediff(OAGene_HediffDefOf.OAGene_Hediff_ExperienceSnowstorm);
            }
        }
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