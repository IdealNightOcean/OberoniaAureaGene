using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public static class SnowstormUtility
{
    private static IntRange IceStormDelay = new(180000, 300000); //3~5天
    private static IntRange TempChangeCount = new(1, 3);
    private static IntRange TempChangeDelay = new(60000, 120000); //1~2天
    private static IntRange TempChangeInterval = new(90000, 120000); //1.5~2天
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
    public static void InitExtremeSnowstormWorld(Map ownerMap, int duration)
    {
        if (Rand.Bool)
        {
            IncidentParms iceParms = new()
            {
                target = ownerMap
            };
            Find.Storyteller.incidentQueue.Add(OAGene_IncidentDefOf.OAGene_ExtremeIceStorm, Find.TickManager.TicksGame + IceStormDelay.RandomInRange, iceParms);
        }
        TryQueueTempChengeIncident(ownerMap, duration);
    }
    public static void InitExtremeSnowstormLocal(Map map, int duration)
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
    public static void EndExtremeSnowstormLocal(Map map)
    {
        if (map == null)
        {
            return;
        }
        map.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowGentle);
        TryGiveEndSnowstormThought(map);
    }

    public static void TryQueueTempChengeIncident(Map ownerMap, int duration)
    {
        int delay = TempChangeDelay.RandomInRange;
        int count = TempChangeCount.RandomInRange;
        IncidentParms parms = new()
        {
            target = ownerMap
        };
        for (int i = 0; i < count; i++)
        {
            IncidentDef incidentDef = Rand.Bool ? OAGene_IncidentDefOf.OAGene_SnowstormWarm : OAGene_IncidentDefOf.OAGene_SnowstormCold;
            Find.Storyteller.incidentQueue.Add(incidentDef, Find.TickManager.TicksGame + delay, parms);
            delay += TempChangeInterval.RandomInRange;
            if (delay < duration - 30000)
            {
                return;
            }
        }
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