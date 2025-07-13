using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IceCrystalFlower : Plant
{
    protected const int MaxChildFlowerCount = 4;
    protected const int MaxAdjacentFlowerCount = 6;
    protected const int SpreadInterval = 15000;

    protected const float HeatPerLong = -550f / 3f;

    protected IceCrystalFlower parentFlower;

    protected int childFlowerCount;
    protected int adjFlowerCount;

    protected int ticksToSpread = SpreadInterval;
    protected int ticksToDieOutdoor = 60000;



    public void Notify_FirstSpawn(IceCrystalFlower parentFlower = null)
    {
        growthInt = 1f;

        Map map = Map;
        IntVec3 pos = Position;
        if (parentFlower is not null)
        {
            this.parentFlower = parentFlower;
            this.parentFlower.childFlowerCount++;
        }
        foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(pos))
        {
            if (!c.InBounds(map))
            {
                continue;
            }
            foreach (IceCrystalFlower t in c.GetThingList(map).OfType<IceCrystalFlower>())
            {
                t.adjFlowerCount++;
            }
        }
    }
    public void Notify_Despawn()
    {
        Map map = Map;
        IntVec3 pos = Position;
        if (parentFlower is not null)
        {
            parentFlower.childFlowerCount = Mathf.Max(childFlowerCount - 1, 0);
        }
        foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(pos))
        {
            if (!c.InBounds(map))
            {
                continue;
            }
            foreach (IceCrystalFlower t in c.GetThingList(map).OfType<IceCrystalFlower>())
            {
                t.adjFlowerCount = Mathf.Max(t.adjFlowerCount - 1, 0);
            }
        }
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        Notify_Despawn();
        base.DeSpawn(mode);
    }

    public override void TickLong()
    {
        base.TickLong();
        if (!Spawned)
        {
            return;
        }

        ticksToSpread -= 1000;
        if (ticksToSpread <= 0)
        {
            TrySpawnNewFlower();
            ticksToSpread = SpreadInterval;
        }

        Room room = Position.GetRoom(Map);
        if (room is null || room.UsesOutdoorTemperature)
        {
            ticksToDieOutdoor -= 1000;
            if (ticksToDieOutdoor <= 0)
            {
                ticksToDieOutdoor = 60000;
                Kill();
                return;
            }
        }
        else
        {
            ticksToDieOutdoor = 60000;
        }

        if (AmbientTemperature > -21f)
        {
            GenTemperature.PushHeat(Position, Map, HeatPerLong);
        }
    }
    protected void TrySpawnNewFlower()
    {
        if (!SnowstormUtility.IsSnowExtremeWeather(Map))
        {
            return;
        }
        if (childFlowerCount >= MaxChildFlowerCount || adjFlowerCount >= MaxAdjacentFlowerCount)
        {
            return;
        }
        if (TryFindNewFlowerCell(Map, Position, out IntVec3 targetCell))
        {
            IceCrystalFlower flower = (IceCrystalFlower)GenSpawn.Spawn(Snowstorm_ThingDefOf.OAGene_Plant_IceCrystalFlower, targetCell, Map);
            flower?.Notify_FirstSpawn(this);
        }
    }

    protected static bool TryFindNewFlowerCell(Map map, IntVec3 center, out IntVec3 targetCell)
    {
        targetCell = IntVec3.Invalid;

        List<IntVec3> validCells = [];
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, 1.9f, false))
        {
            if (!CellValidator(cell))
            {
                continue;
            }
            if (cell.GetRoof(map)?.isThickRoof ?? false)
            {
                validCells.Add(cell);
                continue;
            }
            foreach (IntVec3 cell2 in GenRadial.RadialCellsAround(cell, 3.9f, false))
            {
                if (cell2.InBounds(map) && cell2.GetSnowDepth(map) > 0.1f)
                {
                    validCells.Add(cell);
                    break;
                }
            }
        }
        if (!validCells.Any())
        {
            return false;
        }
        targetCell = validCells.RandomElement();
        return true;

        bool CellValidator(IntVec3 c)
        {
            if (!c.InBounds(map) || c.Impassable(map))
            {
                return false;
            }
            if (c.GetThingList(map).Count != 0)
            {
                return false;
            }
            Room room = c.GetRoom(map);
            if (room is null || room.UsesOutdoorTemperature)
            {
                return false;
            }
            return true;
        }

    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref parentFlower, "parentFlower");
        Scribe_Values.Look(ref childFlowerCount, "childFlowerCount", 0);
        Scribe_Values.Look(ref adjFlowerCount, "adjFlowerCount", 0);
        Scribe_Values.Look(ref ticksToSpread, "ticksToSpread", 0);
        Scribe_Values.Look(ref ticksToDieOutdoor, "ticksToDieOutdoor", 0);
    }

}
