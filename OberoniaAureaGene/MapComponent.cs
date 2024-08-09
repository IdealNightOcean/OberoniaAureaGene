using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;
using OberoniaAurea_Frame;

namespace OberoniaAureaGene;

public class MapComponent_OberoniaAureaGene : MapComponent
{
    public int ticksRemaining;
    public int cachedEnemiesCount;
    public int cachedHostileSitesCount;

    public MapComponent_OberoniaAureaGene(Map map) : base(map)
    { }

    public override void MapComponentTick()
    {
        if (!ModsConfig.IdeologyActive)
        {
            return;
        }
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            PeriodicCheck();
            ticksRemaining = 15000;
        }
    }

    public void QuickCheck()
    {
        ticksRemaining = 600;
    }

    private void PeriodicCheck()
    {
        cachedEnemiesCount = OberoniaAureaFrameUtility.EnemiesCountOfFactionOnMap(map, Faction.OfPlayer);
        cachedHostileSitesCount = HostileCountOfFactionOnWorld(map.Tile, Faction.OfPlayer, 8f);
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref cachedEnemiesCount, "cachedEnemiesCount", 0);
        Scribe_Values.Look(ref cachedHostileSitesCount, "cachedHostileSitesCount", 0);
    }

    public static int HostileCountOfFactionOnWorld(int tile, Faction faction, float maxTileDistance)
    {
        WorldGrid worldGrid = Find.WorldGrid;
        var potentiallyDangerous = Find.WorldObjects.AllWorldObjects.Where(w => w.Spawned && faction.HostileTo(w.Faction) && worldGrid.ApproxDistanceInTiles(tile, w.Tile) < maxTileDistance);
        return potentiallyDangerous.Count();
    }
}
