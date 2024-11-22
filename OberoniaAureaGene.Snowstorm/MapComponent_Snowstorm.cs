using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class MapComponent_Snowstorm : MapComponent
{
    public List<Comp_SnowyCrystalTree> snowyCrystalTreeComps = [];
    public List<Building_IceCrystalCollector> crystalCollectors = [];
    public int SnowyCrystalTreeCount => snowyCrystalTreeComps.Count;
    public MapComponent_Snowstorm(Map map) : base(map) { }

    public void Notyfy_CollectorSpawn(Building_IceCrystalCollector checker)
    {
        if (!crystalCollectors.Contains(checker))
        {
            RecalculateNearCollector(checker, false);
            crystalCollectors.Add(checker);      
        }
    }
    public void Notyfy_CollectorDespawn(Building_IceCrystalCollector checker)
    {
        if (crystalCollectors.Contains(checker))
        {
            crystalCollectors.Remove(checker);
            RecalculateNearCollector(checker, true);
        }
    }
    protected void RecalculateNearCollector(Building_IceCrystalCollector checker, bool despawn = false)
    {
        Map map = base.map;
        IntVec3 position = checker.Position;
        IEnumerable<Building_IceCrystalCollector> mapCollectors = crystalCollectors.Where(mapCollector);
        if (despawn)
        {
            foreach (Building_IceCrystalCollector collector in mapCollectors)
            {
                collector.nearCollectorCount = Math.Max(0, collector.nearCollectorCount - 1);
            }
            checker.nearCollectorCount = 0;
        }
        else
        {
            foreach (Building_IceCrystalCollector collector in mapCollectors)
            {
                checker.nearCollectorCount++;
                collector.nearCollectorCount++;
            }
        }

        bool mapCollector(Thing t)
        {
            if (!t.Spawned)
            {
                return false;
            }
            if (t.Position.DistanceTo(position) > 14.9f)
            {
                return false;
            }
            return true;
        }
    }

}
