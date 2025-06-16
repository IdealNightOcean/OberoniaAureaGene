using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class WeatherEvent_SnowStormBreakRoof : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;

    protected static readonly IntRange AfftectRoomRange = new(1, 3);
    protected static readonly FloatRange AfftectRoofRange = new(0.25f, 0.8f);

    protected static readonly List<TargetInfo> LookTargetCells = [];

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
        if (!OberoniaAureaGene_Settings.SnowstormBreakRoof || Rand.Chance(0.8f))
        {
            return;
        }
        List<Room> potentialRooms = map.regionGrid.AllRooms.Where(r => !r.IsDoorway).InRandomOrder().Take(AfftectRoomRange.RandomInRange).ToList();
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
                foreach (IntVec3 roofCell in targetRoofs)
                {
                    roofGrid.SetRoof(roofCell, null);
                }
            }
        }
        if (affected)
        {
            Messages.Message("OAGene_MessageSnowStormBreakRoof".Translate(), new LookTargets(LookTargetCells), MessageTypeDefOf.NegativeEvent);
        }

        bool ValidRoof(IntVec3 c)
        {
            RoofDef roofDef = roofGrid.RoofAt(c);
            if (roofDef == null)
            {
                return false;
            }
            if (roofDef.isNatural && !OberoniaAureaGene_Settings.SnowstormBreakNaturalRoof)
            {
                return false;
            }
            if (roofDef.isThickRoof && !OberoniaAureaGene_Settings.SnowstormBreakThickRoof)
            {
                return false;
            }
            return !OAGeneUtility.WithinRangeOfRoofHolder(c, map, OberoniaAureaGene_Settings.ColumnProtectRadius);
        }
    }
}
