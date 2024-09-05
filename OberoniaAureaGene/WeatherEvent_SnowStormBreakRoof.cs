using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

public class WeatherEvent_SnowStormBreakRoof : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;

    protected static IntRange AfftectRoomRange = new(1, 3);
    protected static FloatRange AfftectRoofRange = new(0.05f, 0.55f);

    public static readonly List<TargetInfo> LookTargetCells = [];

    public WeatherEvent_SnowStormBreakRoof(Map map) : base(map)
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
        if (Rand.Chance(0.8f))
        {
            return;
        }
        List<Room> potentialRooms = map.regionGrid.allRooms.InRandomOrder().Take(AfftectRoomRange.RandomInRange).ToList();
        if (!potentialRooms.Any())
        {
            return;
        }
        RoofGrid roofGrid = map.roofGrid;
        IEnumerable<IntVec3> potentialRoofs;
        IEnumerable<IntVec3> targetRoofs;
        LookTargetCells.Clear();
        int afftectRoofCount;
        for (int i = 0; i < potentialRooms.Count; i++)
        {
            Room room = potentialRooms[i];
            potentialRoofs = room.Cells.Where(ValidRoof).InRandomOrder();
            afftectRoofCount = (int)(potentialRoofs.Count() * AfftectRoofRange.RandomInRange);
            targetRoofs = potentialRoofs.Take(afftectRoofCount);
            RoofCollapserImmediate.DropRoofInCells(targetRoofs, map);
            LookTargetCells.Add(new TargetInfo(targetRoofs.RandomElement(), map));
        }
      
        LookTargets lookTargets = new(LookTargetCells);
        Messages.Message("OAGene_SnowStormBreakRoof".Translate(), lookTargets, MessageTypeDefOf.NegativeEvent);
        bool ValidRoof(IntVec3 c)
        {
            RoofDef roofDef = roofGrid.RoofAt(c);
            return roofDef != null && !roofDef.isNatural && !roofDef.isThickRoof;
        }
    }
}
