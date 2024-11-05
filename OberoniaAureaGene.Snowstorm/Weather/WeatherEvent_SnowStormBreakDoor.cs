using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WeatherEvent_SnowStormBreakDoor : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;

    protected static readonly List<TargetInfo> LookTargetCells = [];

    public WeatherEvent_SnowStormBreakDoor(Map map) : base(map) { }
    public override void WeatherEventTick() { }

    public override void FireEvent()
    {
        TryFireEvent(map);
        expired = true;
    }

    protected static void TryFireEvent(Map map)
    {
        if (!OAGene_SnowstormSettings.SnowstormBreakDoor)
        {
            return;
        }
        if (Rand.Chance(0.8f))
        {
            return;
        }
        IEnumerable<Building_Door> potentialDoors = map.listerBuildings.AllColonistBuildingsOfType<Building_Door>().Where(d => ValidDoor(d, map));
        if (!potentialDoors.Any())
        {
            return;
        }
        List<Building_Door> targetDoors = potentialDoors.Take(Rand.Bool ? 1 : 2).ToList();
        LookTargetCells.Clear();
        MethodInfo DoorOpenInfo = typeof(Building_Door).GetMethod("DoorOpen", ReflectionUtility.InstanceAttr);
        foreach (Building_Door door in targetDoors)
        {
            LookTargetCells.Add(new TargetInfo(door.Position, map));
            DoorOpenInfo.Invoke(door, parameters: [110]);
            ReflectionUtility.SetFieldValue(door, "holdOpenInt", true);
            door.HitPoints -= 60;
            if (door.HitPoints <= 0 && !door.Destroyed)
            {
                door.Destroy();
            }
        }
        Messages.Message("OAGene_MessageSnowstormBreakDoor".Translate(), new LookTargets(LookTargetCells), MessageTypeDefOf.NegativeEvent);
    }

    protected static bool ValidDoor(Building_Door door, Map map)
    {
        if (door.HitPoints > 300)
        {
            return false;
        }
        if (door.Open)
        {
            return false;
        }
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(door.Position, 1.9f, true))
        {
            if (cell.UsesOutdoorTemperature(map))
            {
                return true;
            }
        }
        return false;
    }
}
