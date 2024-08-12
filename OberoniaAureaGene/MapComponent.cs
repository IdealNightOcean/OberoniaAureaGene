using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

public class MapComponent_OberoniaAureaGene : MapComponent
{
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
        if (!ModsConfig.IdeologyActive)
        {
            return;
        }
        EnemyCheckTick();
        RaidCheckTick();
    }
    protected void EnemyCheckTick()
    {
        enemyCheckTicks--;
        if (enemyCheckTicks <= 0)
        {
            PeriodicEnemyCheck();
            enemyCheckTicks = cachedEnemiesCount > 0 ? 2500 : 15000;
        }
    }
    protected void RaidCheckTick()
    {
        raidCheckTicks--;
        if (raidCheckTicks <= 0)
        {
            TryExcuteRaid();
            raidCheckTicks = 300000;
        }
    }

    protected void RecacheHegemonicFlagCount()
    {
        cachedHegemonicFlagCount = map.listerBuildings.allBuildingsColonist.Where(b => b.def == OberoniaAureaGeneDefOf.OAGene_HegemonicFlag).Count();
    }
    public void QuickEnemyCheck()
    {
        enemyCheckTicks = 600;
    }

    private void PeriodicEnemyCheck()
    {
        if (map.IsPlayerHome)
        {
            cachedEnemiesCount = OberoniaAureaFrameUtility.EnemiesCountOfPlayerOnMap(map);
            cachedHostileSitesCount = HostileCountOfFactionOnWorld(map.Tile, Faction.OfPlayer, 8f);
        }
        else
        {
            cachedEnemiesCount = 0;
            cachedHostileSitesCount = 0;
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
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref enemyCheckTicks, "enemyCheckTicks", 0);
        Scribe_Values.Look(ref cachedEnemiesCount, "cachedEnemiesCount", 0);
        Scribe_Values.Look(ref cachedHostileSitesCount, "cachedHostileSitesCount", 0);

        Scribe_Values.Look(ref raidCheckTicks, "raidCheckTicks", 0);
        Scribe_Values.Look(ref cachedHegemonicFlagCount, "cachedHegemonicFlagCount", 0);
    }

    public static int HostileCountOfFactionOnWorld(int tile, Faction faction, float maxTileDistance)
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
