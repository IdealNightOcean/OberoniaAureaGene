using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene;

public class MapComponent_OberoniaAureaGene : MapComponent
{
    protected int enemyCheckTicks;
    public int cachedEnemiesCount;
    protected int cachedHostileSitesCount;
    public bool HasHostileSites => cachedHostileSitesCount > 0;

    protected int raidCheckTicks;

    private bool HasHegemonicFlag => map.listerThings.AnyThingWithDef(OAGene_MiscDefOf.OAGene_HegemonicFlag);

    public MapComponent_OberoniaAureaGene(Map map) : base(map) { }

    public override void MapComponentTick()
    {
        if (ModsConfig.IdeologyActive)
        {
            EnemyCheckTick();
            RaidCheckTick();
        }
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
            cachedEnemiesCount = map.ThreatsCountOfPlayer();
            cachedHostileSitesCount = HostileSitesCountOfPlayer(map.Tile, 6f);
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
        if (raidCheckTicks-- <= 0)
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
                forced = true,
                faction = Find.FactionManager.RandomRaidableEnemyFaction(allowNonHumanlike: false),
            };
            try
            {
                OAFrame_MiscUtility.TryFireIncidentNow(IncidentDefOf.RaidEnemy, incidentParms);
            }
            catch
            {
                Log.Error("[OAGene] Attempt to trigger hegemonic flag raid failed.");
            }
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref enemyCheckTicks, "enemyCheckTicks", 0);
        Scribe_Values.Look(ref cachedEnemiesCount, "cachedEnemiesCount", 0);
        Scribe_Values.Look(ref cachedHostileSitesCount, "cachedHostileSitesCount", 0);

        Scribe_Values.Look(ref raidCheckTicks, "raidCheckTicks", 0);
    }

    private static int HostileSitesCountOfPlayer(PlanetTile tile, float maxTileDistance)
    {
        if (!tile.Valid)
        {
            return 0;
        }
        PlanetLayer layer = tile.Layer;
        WorldGrid worldGrid = Find.WorldGrid;
        Faction ofPlayer = Faction.OfPlayer;

        int sitesCount = 0;
        foreach (WorldObject w in Find.WorldObjects.AllWorldObjects)
        {
            if (!w.Tile.Valid || w.Tile.Layer != layer || worldGrid.ApproxDistanceInTiles(tile, w.Tile) > maxTileDistance)
            {
                continue;
            }
            if (ofPlayer.HostileTo(w.Faction))
            {
                sitesCount++;
            }
        }

        return sitesCount;
    }
}
