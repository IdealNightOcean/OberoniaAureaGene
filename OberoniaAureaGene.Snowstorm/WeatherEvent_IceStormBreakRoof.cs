using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class WeatherEvent_IceStormBreakRoof : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;

    protected static readonly IntRange AfftectRoomRange = new(4, 6);
    protected static readonly FloatRange AfftectRoofRange = new(0.05f, 0.2f);

    protected static readonly List<TargetInfo> LookTargetCells = [];

    public WeatherEvent_IceStormBreakRoof(Map map) : base(map)
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
        if (!OAGene_SnowstormSettings.IceStormBreakRoof)
        {
            return;
        }
        List<Room> potentialRooms = map.regionGrid.allRooms.Where(r => !r.IsDoorway).InRandomOrder().Take(AfftectRoomRange.RandomInRange).ToList();
        if (!potentialRooms.Any())
        {
            return;
        }
        RoofGrid roofGrid = map.roofGrid;
        List<IntVec3> potentialRoofs;
        List<IntVec3> targetRoofs;
        LookTargetCells.Clear();
        int afftectRoofCount;
        bool affected = false;
        for (int i = 0; i < potentialRooms.Count; i++)
        {
            Room room = potentialRooms[i];
            potentialRoofs = room.Cells.Where(ValidRoof).InRandomOrder().ToList(); //所有可能受影响的屋顶
            afftectRoofCount = (int)(potentialRoofs.Count() * AfftectRoofRange.RandomInRange); //受影响的屋顶的个数
            afftectRoofCount = Mathf.Max(30, afftectRoofCount);
            targetRoofs = potentialRoofs.Take(afftectRoofCount).ToList(); //受影响的屋顶
            if (targetRoofs.Any())
            {
                //选取第一个受影响的屋顶作为LookTarget
                affected = true;
                IntVec3 lookCell = targetRoofs.First();
                LookTargetCells.Add(new TargetInfo(lookCell, map));
                RoofCollapserImmediate.DropRoofInCells(targetRoofs, map);
            }
        }
        if (affected)
        {
            Messages.Message("OAGene_MessageIceStormBreakRoof".Translate(), new LookTargets(LookTargetCells), MessageTypeDefOf.NegativeEvent);
        }

        bool ValidRoof(IntVec3 c)
        {
            RoofDef roofDef = roofGrid.RoofAt(c);
            if (roofDef == null)
            {
                return false;
            }
            if (roofDef.isNatural && !OAGene_SnowstormSettings.IceStormBreakNaturalRoof)
            {
                return false;
            }
            if (roofDef.isThickRoof && !OAGene_SnowstormSettings.IceStormBreakThickRoof)
            {
                return false;
            }
            return !OAGeneUtility.WithinRangeOfRoofHolder(c, map, 3.9f);
        }
    }
}
