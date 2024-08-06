using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OberoniaAureaGene;

public class MapComponent_OberoniaAureaGene : MapComponent
{
    public int ticksRemaining;
    public int cachedEnemiesCount;
    public int cachedHostileSitesCount;

    public MapComponent_OberoniaAureaGene(Map map):base(map)
    { }

    public override void MapComponentTick()
    {
        if(!ModsConfig.IdeologyActive)
        {
            return;
        }
        ticksRemaining--;
        if(ticksRemaining<0)
        {
            PeriodicCheck();
            ticksRemaining = 30000;
        }

    }

    private void PeriodicCheck()
    {
        cachedEnemiesCount = EnemiesCountOfFactionOnMap(map,Faction.OfPlayer);
        cachedHostileSitesCount = HostileCountOfFactionOnWorld(map.Tile, Faction.OfPlayer, 8f);
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref cachedEnemiesCount, "cachedEnemiesCount", 0);
        Scribe_Values.Look(ref cachedHostileSitesCount, "cachedHostileSitesCount", 0);
    }

    public static int HostileCountOfFactionOnWorld(int tile, Faction faction,float maxTileDistance) //map上是否有faction派系的敌人
    {
        WorldGrid worldGrid = Find.WorldGrid;
        var potentiallyDangerous = Find.WorldObjects.AllWorldObjects.Where(w => w.Spawned && faction.HostileTo(w.Faction) && worldGrid.ApproxDistanceInTiles(tile, w.Tile) < maxTileDistance);
        return potentiallyDangerous.Count();
    }
    public static int EnemiesCountOfFactionOnMap(Map map, Faction faction) //map上是否有faction派系的敌人
    {
        var potentiallyDangerous = map.mapPawns.AllPawnsSpawned.Where(p => !p.Dead && !p.IsPrisoner && !p.Downed && !p.InContainerEnclosed).ToArray();
        var hostileFactions = potentiallyDangerous.Where(p => p.Faction != null).Select(p => p.Faction).Where(f => f.HostileTo(faction)).ToArray();
        return hostileFactions.Count();
    }
}
