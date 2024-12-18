using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public static class SnowstormUtility
{
    private static readonly IntRange IceOrStarryDelay = new(180000, 300000); //3~5天

    private static readonly IntRange RaidCountI = new(-1, 2);
    private static readonly IntRange RaidCountII = new(1, 2);
    private static readonly IntRange RaidDelay = new(60000, 120000); //1~2天
    private static readonly IntRange RaidInterval = new(90000, 120000); //1~2天

    private static readonly IntRange TempChangeCount = new(1, 3);
    private static readonly IntRange TempChangeDelay = new(60000, 120000); //1~2天
    private static readonly IntRange TempChangeInterval = new(90000, 120000); //1.5~2天

    private static readonly IntRange TraderCount = new(1, 2);
    private static readonly IntRange TraderDelay = new(15000, 30000); //6~12小时
    private static readonly IntRange TraderInterval = new(15000, 30000); //6~12小时

    public static GameCondition_ExtremeSnowstorm SnowstormCondition => Find.World.gameConditionManager.GetActiveCondition<GameCondition_ExtremeSnowstorm>();
    public static bool IsSnowExtremeWeather(Map map) //是否为极端暴风雪（包括冰晶暴风雪）天气
    {
        if (map == null)
        {
            return false;
        }
        return map.weatherManager.curWeather == Snowstorm_MiscDefOf.OAGene_SnowExtreme || map.weatherManager.curWeather == Snowstorm_MiscDefOf.OAGene_IceSnowExtreme;
    }
    public static void InitExtremeSnowstorm_World(int duration)
    {
        if (!TryStarryNight() && Rand.Bool)
        {
            AddNewWorldIncident(Snowstorm_IncidentDefOf.OAGene_ExtremeIceStorm, IceOrStarryDelay.RandomInRange);
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
        AddNewWorldIncident(Snowstorm_IncidentDefOf.OAGene_StarryNight, IceOrStarryDelay.RandomInRange);
        return true;
    }

    public static void InitExtremeSnowstorm_MainMap(Map mainMap, int duration)
    {
        mainMap ??= Find.AnyPlayerHomeMap;
        if (mainMap == null)
        {
            return;
        }
        mainMap.SnowstormMapComp()?.Notify_SnowstormStart(duration);

        //骤冷丨骤暖
        TryQueueTempChengeIncident(mainMap, duration);
        //暴风雪破墙袭击
        if (OAGene_SnowstormSettings.AllowSnowstormMaliciousRaid)
        {
            TryInitSnowstormRaid(mainMap, duration);
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
        map.SnowstormMapComp()?.Notify_SnowstormStart(duration);
    }
    public static void EndExtremeSnowstorm_World()
    {
        Find.World.gameConditionManager.GetActiveCondition<GameCondition_Icestorm>()?.EndSlience();
    }
    public static void EndExtremeSnowstorm_MainMap(Map mainMap)
    {
        mainMap ??= Find.AnyPlayerHomeMap;
        if (mainMap == null)
        {
            return;
        }
        mainMap.SnowstormMapComp()?.Notify_SnowstormEnd();
        TryInitAfterSnowstormIncident(mainMap);
    }
    public static void EndExtremeSnowstorm_AllMaps(Map map, bool slience = false)
    {
        if (map == null)
        {
            return;
        }
        map.SnowstormMapComp()?.Notify_SnowstormEnd();
        TryGiveEndSnowstormHediffAndThought(map);
    }
    //骤冷丨骤暖事件 (mainMap)
    public static void TryQueueTempChengeIncident(Map mainMap, int duration)
    {
        int delayTicks = TempChangeDelay.RandomInRange;
        int count = TempChangeCount.RandomInRange;

        for (int i = 0; i < count; i++)
        {
            IncidentDef incidentDef = Rand.Bool ? Snowstorm_IncidentDefOf.OAGene_SnowstormWarm : Snowstorm_IncidentDefOf.OAGene_SnowstormCold;
            AddNewMapIncident(incidentDef, mainMap, delayTicks);
            delayTicks += TempChangeInterval.RandomInRange;
            if (delayTicks < duration - 90000)
            {
                return;
            }
        }
    }

    //暴风雪中的事件 (mainMap)
    public static void TryInitSnowstormIncident(Map mainMap, int duration)
    {
        int years = GenDate.YearsPassed;
        //雪雾弥漫
        if (years >= 3 && Rand.Chance(0.25f))
        {
            AddNewMapIncident(Snowstorm_IncidentDefOf.OAGene_SnowstormFog, mainMap, Rand.RangeInclusive(180000, 360000));
        }
        //挣扎者
        if (Rand.Chance(0.4f))
        {
            AddNewMapIncident(Snowstorm_IncidentDefOf.OAGene_SnowstormStrugglers, mainMap, Rand.RangeInclusive(120000, 240000));
        }
        //遇难商人
        if (Rand.Chance(0.6f))
        {
            AddNewMapIncident(Snowstorm_IncidentDefOf.OAGene_AffectedMerchant, mainMap, Rand.RangeInclusive(180000, 300000));
        }
        //敲击兽
        if (Rand.Chance(0.2f))
        {
            AddNewMapIncident(Snowstorm_IncidentDefOf.OAGene_SnowstormThrumboWanderIn, mainMap, Rand.RangeInclusive(30000, duration - 30000));
        }
        //信号塔
        if (Rand.Chance(0.4f))
        {
            AddNewMapIncident(Snowstorm_IncidentDefOf.OAGene_CommunicationTowerCollapse, mainMap, Rand.RangeInclusive(120000, 180000));
        }
    }

    //暴风雪破墙袭击 (mainMap)
    public static void TryInitSnowstormRaid(Map mainMap, int duration)
    {
        if (GenDate.DaysPassed < 60)
        {
            return;
        }
        int raidCount = GenDate.YearsPassed < 5 ? RaidCountI.RandomInRange : RaidCountII.RandomInRange;
        if (raidCount <= 0)
        {
            return;
        }

        int delayTicks = RaidDelay.RandomInRange;
        for (int i = 0; i < raidCount; i++)
        {
            AddNewMapIncident(Snowstorm_IncidentDefOf.OAGene_SnowstormMaliceRaid, mainMap, delayTicks);
            delayTicks += RaidInterval.RandomInRange;
            if (delayTicks > duration - 30000)
            {
                break;
            }
        }
    }
    //暴风雪中的恶意 (mainMap)
    public static void TryInitSnowstormMalice(Map mainMap)
    {
        if (GenDate.YearsPassed < 4 || Rand.Chance(0.66f))
        {
            return;
        }
        IncidentDef incidentDef;
        if (ModsConfig.RoyaltyActive)
        {
            incidentDef = Rand.Bool ? Snowstorm_IncidentDefOf.OAGene_SnowstormRaidSource : Snowstorm_IncidentDefOf.OAGene_SnowstormClimateAdjuster;
        }
        else
        {
            incidentDef = Snowstorm_IncidentDefOf.OAGene_SnowstormRaidSource;
        }
        int delayTicks = Rand.RangeInclusive(120000, 180000);
        AddNewMapIncident(incidentDef, mainMap, delayTicks);
    }

    //暴风雪恶意袭击可用的派系
    public static Faction RandomSnowstormMaliceRaidableFaction(Map map)
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
            tempParms.raidStrategy = Snowstorm_MiscDefOf.OAGene_SnowstormImmediateAttackBreaching;
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
        List<Pawn> pawns = map.mapPawns.FreeColonistsSpawned;
        foreach (Pawn pawn in pawns)
        {
            pawn.needs.mood?.thoughts.memories.TryGainMemory(Snowstorm_ThoughtDefOf.OAGene_Thought_SnowstormEnd);
            pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_ExperienceSnowstorm);
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
            AddNewMapIncident(Snowstorm_IncidentDefOf.OAGene_AfterSnowstormTraderCaravanArrival, mainMap, delayTicks);
            delayTicks += TraderInterval.RandomInRange;
        }

        if (Rand.Chance(0.4f))
        {
            delayTicks = Rand.RangeInclusive(10000, 50000);
            AddNewMapIncident(Snowstorm_IncidentDefOf.OAGene_SnowstormSurvivorJoins, mainMap, delayTicks);
        }
    }
    public static void AddNewWorldIncident(IncidentDef incidentDef, int delayTicks)
    {
        IncidentParms parms = new()
        {
            forced = true,
            target = Find.World
        };
        OAFrame_MiscUtility.AddNewQueuedIncident(incidentDef, delayTicks, parms);
    }

    public static void AddNewMapIncident(IncidentDef incidentDef, Map targetMap, int delayTicks)
    {
        if (targetMap == null)
        {
            Log.Error("Try add a map incident, but targetMap is NULL");
            return;
        }
        IncidentParms parms = new()
        {
            forced = true,
            target = targetMap
        };
        OAFrame_MiscUtility.AddNewQueuedIncident(incidentDef, delayTicks, parms);
    }
}