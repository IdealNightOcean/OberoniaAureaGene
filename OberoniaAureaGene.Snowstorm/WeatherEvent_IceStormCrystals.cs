using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WeatherEvent_IceStormCrystals : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;

    protected static readonly IntRange CrystalsCountRange = new(8, 20);

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
        TryFindCell(out IntVec3 spawnCenter, map);
        if (!spawnCenter.IsValid)
        {
            return;
        }
        int spawnCounts = CrystalsCountRange.RandomInRange;
        for (int i = 0; i < spawnCounts; i++)
        {
            Thing t = ThingMaker.MakeThing(Snowstrom_MiscDefOf.OAGene_IceStormCrystal);
            GenPlace.TryPlaceThing(t, spawnCenter, map, ThingPlaceMode.Near, delegate (Thing thing, int count)
            {
                PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
            }, null, t.def.defaultPlacingRot);
        }
    }
    protected static bool TryFindCell(out IntVec3 cell, Map map)
    {
        int maxMineables = CrystalsCountRange.max;
        return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.MeteoriteIncoming, map, out cell, 10, default, -1, allowRoofedCells: true, allowCellsWithItems: false, allowCellsWithBuildings: false, colonyReachable: false, avoidColonistsIfExplosive: true, alwaysAvoidColonists: true, delegate (IntVec3 x)
        {
            int num = Mathf.CeilToInt(Mathf.Sqrt(maxMineables)) + 2;
            CellRect other = CellRect.CenteredOn(x, num, num);
            int validCellCount = 0;
            foreach (IntVec3 item in other)
            {
                if (item.InBounds(map) && item.Standable(map))
                {
                    validCellCount++;
                }
            }
            return validCellCount >= maxMineables;
        });
    }
}
