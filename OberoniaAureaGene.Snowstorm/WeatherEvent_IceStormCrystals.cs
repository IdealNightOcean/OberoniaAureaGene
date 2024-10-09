using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WeatherEvent_IceStormCrystals : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;

    protected static readonly IntRange CrystalsCountRange = new(8,20);

    public WeatherEvent_IceStormCrystals(Map map) : base(map)
    { }
    public override void WeatherEventTick()
    { }

    public override void FireEvent()
    {
        TryFireEvent(map);
        expired = true;
    }
    protected static void TryFireEvent(Map map)
    {


    }
    private static bool TryFindCell(out IntVec3 cell, Map map)
    {
        int maxMineables = ThingSetMaker_Meteorite.MineablesCountRange.max;
        return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.MeteoriteIncoming, map, out cell, 10, default(IntVec3), -1, allowRoofedCells: true, allowCellsWithItems: false, allowCellsWithBuildings: false, colonyReachable: false, avoidColonistsIfExplosive: true, alwaysAvoidColonists: true, delegate (IntVec3 x)
        {
            int num = Mathf.CeilToInt(Mathf.Sqrt(maxMineables)) + 2;
            CellRect other = CellRect.CenteredOn(x, num, num);
            int num2 = 0;
            foreach (IntVec3 item in other)
            {
                if (item.InBounds(map) && item.Standable(map))
                {
                    num2++;
                }
            }
            if (ModsConfig.RoyaltyActive)
            {
                foreach (Thing item2 in map.listerThings.ThingsOfDef(ThingDefOf.MonumentMarker))
                {
                    MonumentMarker monumentMarker = item2 as MonumentMarker;
                    if (monumentMarker.AllDone && monumentMarker.sketch.OccupiedRect.ExpandedBy(3).MovedBy(monumentMarker.Position).Overlaps(other))
                    {
                        return false;
                    }
                }
            }
            return num2 >= maxMineables;
        });
    }
}
