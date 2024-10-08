using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public static class SnowstormUtility
{
    private static readonly IntRange IceStormDelay = new(180000, 300000); //3~5天
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
        return map.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_SnowExtreme || map.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_IceSnowExtreme;
    }
    public static bool IsIceStormWeather(Map map) //是否为冰晶暴风雪天气
    {
        return map?.weatherManager.curWeather == OAGene_MiscDefOf.OAGene_IceSnowExtreme;
    }
    public static void InitExtremeSnowstorm_MainMap(Map mainMap, int duration)
    {
        mainMap ??= Find.AnyPlayerHomeMap;
        if (Rand.Bool)
        {
            IncidentParms iceParms = new()
            {
                target = mainMap
            };
            Find.Storyteller.incidentQueue.Add(OAGene_SnowstromDefOf.OAGene_ExtremeIceStorm, Find.TickManager.TicksGame + IceStormDelay.RandomInRange, iceParms);
        }
        TryQueueTempChengeIncident(mainMap, duration);
        TryInitSnowstormRaid(mainMap);
    }
    public static void InitExtremeSnowstorm_AllMaps(Map map, int duration)
    {
        if (map == null)
        {
            return;
        }
        map.weatherManager.TransitionTo(OAGene_MiscDefOf.OAGene_SnowExtreme);
        map.GetOAGeneMapComp()?.Notify_Snow(duration);
        OAGeneUtility.TryBreakPowerPlantWind(map, duration);
    }
    public static void EndExtremeSnowstorm_MainMap(Map mainMap)
    {
        TryInitAfterSnowstormIncident(mainMap);
    }
    public static void EndExtremeSnowstorm_AllMaps(Map map)
    {
        if (map == null)
        {
            return;
        }
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
            IncidentDef incidentDef = Rand.Bool ? OAGene_SnowstromDefOf.OAGene_SnowstormWarm : OAGene_SnowstromDefOf.OAGene_SnowstormCold;
            AddNewIncident(incidentDef, mainMap, delayTicks);
            delayTicks += TempChangeInterval.RandomInRange;
            if (delayTicks < duration - 30000)
            {
                return;
            }
        }
    }

    //暴风雪中的事件 (mainMap)
    public static void TryInitSnowstormIncident(Map mainMap)
    {
        int delayTicks;

        if (Rand.Chance(0.4f))
        {
            delayTicks = new IntRange(120000, 240000).RandomInRange;
            AddNewIncident(OAGene_SnowstromDefOf.OAGene_SnowstromStrugglers, mainMap, delayTicks);

        }
        if (Rand.Chance(0.6f))
        {
            delayTicks = new IntRange(180000, 300000).RandomInRange;
            AddNewIncident(OAGene_SnowstromDefOf.OAGene_AffectedMerchant, mainMap, delayTicks);
        }

    }

    //暴风雪破墙袭击 (mainMap)
    public static void TryInitSnowstormRaid(Map mainMap)
    {
        if (GenDate.DaysPassed < 60)
        {
            return;
        }
        int raidCount = RaidCount.RandomInRange;
        if (raidCount > 0)
        {
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, mainMap);
            parms.forced = true;
            parms.raidStrategy = OAGene_SnowstromDefOf.OAGene_SnowstormImmediateAttackBreaching;

            Faction faction = RandomRaidableEnemyFaction(parms);
            if (faction == null)
            {
                return;
            }
            parms.faction = faction;
            int delayTicks = RaidDelay.RandomInRange;
            for (int i = 0; i < raidCount; i++)
            {
                Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + delayTicks, parms);
                delayTicks += RaidInterval.RandomInRange;
            }
        }
    }
    private static Faction RandomRaidableEnemyFaction(IncidentParms parms = null)
    {
        FactionManager factionManager = Find.FactionManager;
        Faction playerFaction = Faction.OfPlayer;
        //获取可用派系
        (from f in factionManager.GetFactions(allowHidden: false, allowDefeated: false, allowNonHumanlike: false, TechLevel.Undefined)
         where ValidFaction(f)
         select f).TryRandomElement(out Faction faction);

        //如果没有，创建临时海盗派系
        if (faction == null)
        {
            FactionGeneratorParms factionParms = new(FactionDefOf.Pirate, default, true);
            factionParms.ideoGenerationParms = new IdeoGenerationParms(factionParms.factionDef, forceNoExpansionIdeo: false, hidden: true);
            List<FactionRelation> list = [];
            foreach (Faction faction1 in Find.FactionManager.AllFactionsListForReading)
            {
                if (!faction1.def.PermanentlyHostileTo(factionParms.factionDef))
                {
                    if (faction1 == playerFaction)
                    {
                        list.Add(new FactionRelation
                        {
                            other = faction1,
                            kind = FactionRelationKind.Hostile
                        });
                    }
                    else
                    {
                        list.Add(new FactionRelation
                        {
                            other = faction1,
                            kind = FactionRelationKind.Neutral
                        });
                    }
                }
            }
            faction = FactionGenerator.NewGeneratedFactionWithRelations(factionParms, list);
            faction.temporary = true;
            Find.FactionManager.Add(faction);
        }

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
            if (parms != null)
            {
                parms.faction = fa;
                RaidStrategyDef strategyDef = parms.raidStrategy;
                if (strategyDef == null || !strategyDef.Worker.CanUseWith(parms, PawnGroupKindDefOf.Combat))
                {
                    return false;
                }
                if (parms.raidArrivalMode != null)
                {
                    return true;
                }
                return strategyDef.arriveModes?.Any((PawnsArrivalModeDef x) => x.Worker.CanUseWith(parms)) ?? false;
            }
            return true;
        }
    }

    //暴风雪中的恶意 (mainMap)
    public static void TryInitSnowstormMalice(Map mainMap)
    {
        if (GenDate.DaysPassed < 60 || Rand.Chance(0.66f))
        {
            return;
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
                pawn.needs.mood.thoughts.memories.TryGainMemory(OAGene_SnowstromDefOf.OAGene_Thought_SnowstormEnd);
            }
            if (pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                pawn.health.AddHediff(OAGene_SnowstromDefOf.OAGene_Hediff_ExperienceSnowstorm);
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
            AddNewIncident(OAGene_SnowstromDefOf.OAGene_AfterSnowstormTraderCaravanArrival, mainMap, delayTicks);
            delayTicks += TraderInterval.RandomInRange;
        }

        if (Rand.Chance(0.4f))
        {
            delayTicks = new IntRange(10000, 50000).RandomInRange;
            AddNewIncident(OAGene_SnowstromDefOf.OAGene_SnowstormSurvivorJoins, mainMap, delayTicks);
        }
    }

    private static void AddNewIncident(IncidentDef incidentDef, Map targetMap, int delayTicks)
    {
        IncidentParms parms = new()
        {
            target = targetMap
        };
        Find.Storyteller.incidentQueue.Add(incidentDef, Find.TickManager.TicksGame + delayTicks, parms);
    }
}