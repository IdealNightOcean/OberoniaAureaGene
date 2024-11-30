using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public static class SnowstormUtility
{
    private static readonly IntRange IceOrStarryDelay = new(180000, 300000); //3~5天

    private static readonly IntRange RaidCount = new(-1, 2);
    private static readonly IntRange RaidDelay = new(60000, 120000); //1~2天
    private static readonly IntRange RaidInterval = new(90000, 120000); //1~2天

    private static readonly IntRange TempChangeCount = new(1, 3);
    private static readonly IntRange TempChangeDelay = new(60000, 120000); //1~2天
    private static readonly IntRange TempChangeInterval = new(90000, 120000); //1.5~2天

    private static readonly IntRange TraderCount = new(1, 2);
    private static readonly IntRange TraderDelay = new(15000, 30000); //6~12小时
    private static readonly IntRange TraderInterval = new(15000, 30000); //6~12小时
    public static bool IsSnowExtremeWeather(Map map) //是否为极端暴风雪（包括冰晶暴风雪）天气
    {
        if (map == null)
        {
            return false;
        }
        return map.weatherManager.curWeather == Snowstrom_MiscDefOf.OAGene_SnowExtreme || map.weatherManager.curWeather == Snowstrom_MiscDefOf.OAGene_IceSnowExtreme;
    }
    public static bool IsIceStormWeather(Map map) //是否为冰晶暴风雪天气
    {
        return map?.weatherManager.curWeather == Snowstrom_MiscDefOf.OAGene_IceSnowExtreme;
    }
    public static bool IsIceExtremeWeather(Map map) //是否为冰晶天气
    {
        if (map == null)
        {
            return false;
        }
        return map.weatherManager.curWeather == Snowstrom_MiscDefOf.OAGene_IceSnowExtreme || map.weatherManager.curWeather == Snowstrom_MiscDefOf.OAGene_IceRain;
    }
    public static void InitExtremeSnowstorm_World(int duration)
    {
        if (!TryStarryNight() && Rand.Bool)
        {
            AddNewWorldIncident(Snowstrom_IncidentDefOf.OAGene_ExtremeIceStorm, IceOrStarryDelay.RandomInRange);
        }
    }

    public static bool TryStarryNight()
    {
        if (Snowstorm_MiscUtility.SnowstormGameComp.starryNightTriggered)
        {
            return false;
        }
        if (GenDate.YearsPassed < 4)
        {
            return false;
        }
        AddNewWorldIncident(Snowstrom_IncidentDefOf.OAGene_StarryNight, IceOrStarryDelay.RandomInRange);
        return true;
    }

    public static void InitExtremeSnowstorm_MainMap(Map mainMap, int duration)
    {
        mainMap ??= Find.AnyPlayerHomeMap;
        mainMap.SnowstormMapComp()?.Notify_SnowstromStart();

        //骤冷丨骤暖
        TryQueueTempChengeIncident(mainMap, duration);
        //暴风雪破墙袭击
        if (OAGene_SnowstormSettings.AllowSnowstormMaliciousRaid)
        {
            TryInitSnowstormRaid(mainMap);
        }
        //暴风雪中的恶意
        if (OAGene_SnowstormSettings.AllowSnowstormMaliciousSite)
        {
            TryInitSnowstormMalice(mainMap);
        }
        //其它事件
        TryInitSnowstormIncident(mainMap, duration);

    }
    public static void InitExtremeSnowstorm_AllMaps(Map map, int duration)
    {
        if (map == null)
        {
            return;
        }
        map.SnowstormMapComp()?.Notify_SnowstromStart();
        map.weatherManager.TransitionTo(OAGene_MiscDefOf.OAGene_SnowExtreme);
        map.GetOAGeneMapComp()?.Notify_Snow(duration);
        OAGeneUtility.TryBreakPowerPlantWind(map, duration);
    }
    public static void EndExtremeSnowstorm_MainMap(Map mainMap)
    {
        mainMap.SnowstormMapComp()?.Notify_SnowstromEnd();
        mainMap.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowGentle);
        TryInitAfterSnowstormIncident(mainMap);
    }
    public static void EndExtremeSnowstorm_AllMaps(Map map)
    {
        if (map == null)
        {
            return;
        }
        map.SnowstormMapComp()?.Notify_SnowstromEnd();
        map.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowGentle);
        TryGiveEndSnowstormHediffAndThought(map);
    }


    //骤冷丨骤暖事件 (mainMap)
    public static void TryQueueTempChengeIncident(Map mainMap, int duration)
    {
        int delayTicks = TempChangeDelay.RandomInRange;
        int count = TempChangeCount.RandomInRange;

        for (int i = 0; i < count; i++)
        {
            IncidentDef incidentDef = Rand.Bool ? Snowstrom_IncidentDefOf.OAGene_SnowstormWarm : Snowstrom_IncidentDefOf.OAGene_SnowstormCold;
            AddNewIncident(incidentDef, mainMap, delayTicks);
            delayTicks += TempChangeInterval.RandomInRange;
            if (delayTicks < duration - 30000)
            {
                return;
            }
        }
    }

    //暴风雪中的事件 (mainMap)
    public static void TryInitSnowstormIncident(Map mainMap, int duration)
    {
        //挣扎者
        if (Rand.Chance(0.4f))
        {
            AddNewIncident(Snowstrom_IncidentDefOf.OAGene_SnowstromStrugglers, mainMap, Rand.RangeInclusive(120000, 240000));
        }
        //遇难商人
        if (Rand.Chance(0.6f))
        {
            AddNewIncident(Snowstrom_IncidentDefOf.OAGene_AffectedMerchant, mainMap, Rand.RangeInclusive(180000, 300000));
        }
        //敲击兽
        if (Rand.Chance(0.2f))
        {
            AddNewIncident(Snowstrom_IncidentDefOf.OAGene_SnowstornThrumboWanderIn, mainMap, Rand.RangeInclusive(30000, duration - 30000));
        }
        //信号塔
        if (Rand.Chance(0.4f))
        {
            AddNewIncident(Snowstrom_IncidentDefOf.OAGene_CommunicationTowerCollapse, mainMap, Rand.RangeInclusive(120000, 180000));
        }
    }

    //暴风雪破墙袭击 (mainMap)
    public static void TryInitSnowstormRaid(Map mainMap)
    {
        int raidCount = RaidCount.RandomInRange;
        if (GenDate.DaysPassed < 60)
        {
            return;
        }
        if (GenDate.YearsPassed >= 5)
        {
            raidCount = Mathf.Max(raidCount, 1);
        }

        if (raidCount <= 0)
        {
            return;
        }

        int delayTicks = RaidDelay.RandomInRange;
        for (int i = 0; i < raidCount; i++)
        {
            AddNewIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormMaliceRaid, mainMap, delayTicks);
            delayTicks += RaidInterval.RandomInRange;
        }
    }
    //暴风雪中的恶意 (mainMap)
    public static void TryInitSnowstormMalice(Map mainMap)
    {
        if (GenDate.DaysPassed < 60 || Rand.Chance(0.66f))
        {
            return;
        }
        IncidentDef incidentDef;
        if (ModsConfig.RoyaltyActive)
        {
            incidentDef = Rand.Bool ? Snowstrom_IncidentDefOf.OAGene_SnowstormRaidSource : Snowstrom_IncidentDefOf.OAGene_SnowstormClimateAdjuster;
        }
        else
        {
            incidentDef = Snowstrom_IncidentDefOf.OAGene_SnowstormRaidSource;
        }
        int delayTicks = Rand.RangeInclusive(120000, 180000);
        AddNewIncident(incidentDef, mainMap, delayTicks);
    }

    //暴风雪恶意袭击可用的派系
    public static Faction RandomSnowstromMaliceRaidableFaction(Map map)
    {
        FactionManager factionManager = Find.FactionManager;
        Faction playerFaction = Faction.OfPlayer;
        Faction faction = OAFrame_FactionUtility.RandomFactionOfDef(FactionDefOf.Pirate, allowDefeated: false, allowTemporary: true);
        if (faction != null)
        {
            return faction;
        }

        //获取其它可用派系
        (from f in factionManager.GetFactions(allowHidden: true, allowDefeated: false, allowNonHumanlike: false, TechLevel.Undefined, allowTemporary: true)
         where ValidFaction(f)
         select f).TryRandomElement(out faction);

        //如果没有，创建临时海盗派系
        faction ??= OAFrame_FactionUtility.GenerateTempFaction(FactionDefOf.Pirate, FactionRelationKind.Hostile);

        return faction;

        //派系是否可用
        bool ValidFaction(Faction fa)
        {
            if (!fa.HostileTo(playerFaction))
            {
                return false;
            }
            if (fa.def.pawnGroupMakers.NullOrEmpty())
            {
                return false;
            }
            IncidentParms tempParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            tempParms.forced = true;
            tempParms.raidStrategy = Snowstrom_MiscDefOf.OAGene_SnowstormImmediateAttackBreaching;
            tempParms.faction = fa;
            RaidStrategyDef strategyDef = tempParms.raidStrategy;
            if (strategyDef == null || !strategyDef.Worker.CanUseWith(tempParms, PawnGroupKindDefOf.Combat))
            {
                return false;
            }
            if (tempParms.raidArrivalMode != null)
            {
                return true;
            }
            return strategyDef.arriveModes?.Any((PawnsArrivalModeDef x) => x.Worker.CanUseWith(tempParms)) ?? false;
        }
    }
    //暴风雪结束后的心情与buff (allMaps) 
    public static void TryGiveEndSnowstormHediffAndThought(Map map)
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
                pawn.needs.mood.thoughts.memories.TryGainMemory(Snowstrom_ThoughtDefOf.OAGene_Thought_SnowstormEnd);
            }
            if (pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                pawn.health.AddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_ExperienceSnowstorm);
            }
        }
    }

    //暴风雪结束后的事件 (mainMap)
    public static void TryInitAfterSnowstormIncident(Map mainMap)
    {
        int delayTicks;

        int traderCount = TraderCount.RandomInRange;
        delayTicks = TraderDelay.RandomInRange;
        for (int i = 0; i < traderCount; i++)
        {
            AddNewIncident(Snowstrom_IncidentDefOf.OAGene_AfterSnowstormTraderCaravanArrival, mainMap, delayTicks);
            delayTicks += TraderInterval.RandomInRange;
        }

        if (Rand.Chance(0.4f))
        {
            delayTicks = Rand.RangeInclusive(10000, 50000);
            AddNewIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormSurvivorJoins, mainMap, delayTicks);
        }
    }
    public static void AddNewWorldIncident(IncidentDef incidentDef, int delayTicks)
    {
        IncidentParms parms = new()
        {
            target = Find.World
        };
        Find.Storyteller.incidentQueue.Add(incidentDef, Find.TickManager.TicksGame + delayTicks, parms);
    }

    public static void AddNewIncident(IncidentDef incidentDef, Map targetMap, int delayTicks)
    {
        IncidentParms parms = new()
        {
            target = targetMap
        };
        Find.Storyteller.incidentQueue.Add(incidentDef, Find.TickManager.TicksGame + delayTicks, parms);
    }
}