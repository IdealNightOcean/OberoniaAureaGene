using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

public class MapComponent_OberoniaAureaGene : MapComponent
{
    protected int lastSnowTick = -1;
    protected int snowCheckTicks;

    protected int enemyCheckTicks;
    public int cachedEnemiesCount;
    protected int cachedHostileSitesCount;
    public bool HasHostileSites => cachedHostileSitesCount > 0;

    protected int raidCheckTicks;
    protected int cachedHegemonicFlagCount;
    public int HegemonicFlagCount
    {
        get { return cachedHegemonicFlagCount; }
        set { cachedHegemonicFlagCount = value > 0 ? value : 0; }
    }
    public bool HasHegemonicFlag => cachedHegemonicFlagCount > 0;

    public MapComponent_OberoniaAureaGene(Map map) : base(map)
    { }

    public override void MapComponentTick()
    {
        SnowCheckTick();
        if (ModsConfig.IdeologyActive)
        {
            EnemyCheckTick();
            RaidCheckTick();
        }
    }

    //漫长风雪的保底下雪天气
    protected void SnowCheckTick()
    {
        snowCheckTicks--;
        if (snowCheckTicks <= 0)
        {
            snowCheckTicks = CheckSnow() ? 60000 : 15000;
        }
    }
    protected bool CheckSnow()
    {
        if (!map.IsPlayerHome || !map.GameConditionManager.ConditionIsActive(OberoniaAureaGeneDefOf.OAGene_Snowstorm))
        {
            return true;
        }
        WeatherManager weatherManager = map.weatherManager;
        if (weatherManager.curWeather == OAGene_RimWorldDefOf.SnowHard || weatherManager.curWeather == OberoniaAureaGeneDefOf.OAGene_SnowExtreme)
        {
            lastSnowTick = Find.TickManager.TicksGame + 60000;
        }
        if (Find.TickManager.TicksGame - lastSnowTick > 300000 && map.weatherDecider.ForcedWeather == null)
        {
            map.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowHard);
            ReflectionUtility.SetFieldValue(map.weatherDecider, "curWeatherDuration", 60000);
            lastSnowTick = Find.TickManager.TicksGame + 60000;
            return true;
        }
        return false;
    }

    public void Notify_Snow(int snowDuration = 0)
    {
        lastSnowTick = Find.TickManager.TicksGame + snowDuration;
        snowCheckTicks = snowDuration;
    }

    //搜索地图上敌人和大地图敌对据点
    protected void EnemyCheckTick()
    {
        enemyCheckTicks--;
        if (enemyCheckTicks <= 0)
        {
            PeriodicEnemyCheck();
            enemyCheckTicks = cachedEnemiesCount > 0 ? 2500 : 15000;
        }
    }
    private void PeriodicEnemyCheck()
    {
        if (map.IsPlayerHome)
        {
            cachedEnemiesCount = OberoniaAureaFrameUtility.ThreatsCountOfPlayerOnMap(map);
            cachedHostileSitesCount = HostileSitesCountOfFactionOnWorld(map.Tile, Faction.OfPlayer, 8f);
        }
        else
        {
            cachedEnemiesCount = 0;
            cachedHostileSitesCount = 0;
        }
    }
    public void QuickEnemyCheck()
    {
        enemyCheckTicks = 600;
    }

    //霸权旗的周期袭击
    protected void RaidCheckTick()
    {
        raidCheckTicks--;
        if (raidCheckTicks <= 0)
        {
            TryExcuteRaid();
            raidCheckTicks = 300000;
        }
    }
    private void TryExcuteRaid()
    {
        if (!HasHegemonicFlag || !map.IsPlayerHome)
        {
            return;
        }
        if (Rand.Chance(0.4f))
        {
            IncidentParms incidentParms = new()
            {
                target = map,
                faction = Find.FactionManager.RandomRaidableEnemyFaction(allowNonHumanlike: false),
            };
            IncidentDefOf.RaidEnemy.Worker.TryExecute(incidentParms);
        }
    }
    protected void RecacheHegemonicFlagCount()
    {
        cachedHegemonicFlagCount = map.listerBuildings.allBuildingsColonist.Where(b => b.def == OberoniaAureaGeneDefOf.OAGene_HegemonicFlag).Count();
    }


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref lastSnowTick, "lastSnowTick", -1);
        Scribe_Values.Look(ref snowCheckTicks, "snowCheckTicks", 0);

        Scribe_Values.Look(ref enemyCheckTicks, "enemyCheckTicks", 0);
        Scribe_Values.Look(ref cachedEnemiesCount, "cachedEnemiesCount", 0);
        Scribe_Values.Look(ref cachedHostileSitesCount, "cachedHostileSitesCount", 0);

        Scribe_Values.Look(ref raidCheckTicks, "raidCheckTicks", 0);
        Scribe_Values.Look(ref cachedHegemonicFlagCount, "cachedHegemonicFlagCount", 0);
    }

    public static int HostileSitesCountOfFactionOnWorld(int tile, Faction faction, float maxTileDistance)
    {
        if (tile <= 0)
        {
            return 0;
        }
        WorldGrid worldGrid = Find.WorldGrid;
        IEnumerable<WorldObject> potentiallyDangerous = Find.WorldObjects.AllWorldObjects.Where(w => w.Tile > 0 && faction.HostileTo(w.Faction) && worldGrid.ApproxDistanceInTiles(tile, w.Tile) < maxTileDistance);
        return potentiallyDangerous.Count();
    }
}
