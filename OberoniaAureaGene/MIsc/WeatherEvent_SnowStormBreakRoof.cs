using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class WeatherEvent_SnowStormBreakRoof : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;

    protected static readonly IntRange AfftectRoomRange = new(1, 3);
    protected static readonly FloatRange AfftectRoofRange = new(0.05f, 0.55f);

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
        bool affected = false;
        for (int i = 0; i < potentialRooms.Count; i++)
        {
            Room room = potentialRooms[i];
            potentialRoofs = room.Cells.Where(ValidRoof).InRandomOrder(); //所有可能受影响的屋顶
            afftectRoofCount = (int)(potentialRoofs.Count() * AfftectRoofRange.RandomInRange); //受影响的屋顶的个数
            targetRoofs = potentialRoofs.Take(afftectRoofCount); //受影响的屋顶
            if (targetRoofs.Any())
            {
                foreach (IntVec3 roofCell in targetRoofs)
                {
                    roofGrid.SetRoof(roofCell, null);
                }
                affected = true;
                //随机选取一个受影响的屋顶作为LookTarget
                IntVec3 lookCell = targetRoofs.RandomElement();
                LookTargetCells.Add(new TargetInfo(lookCell, map));
            }
        }
        if (affected)
        {
            Messages.Message("OAGene_MessageSnowStormBreakRoof".Translate(), new LookTargets(LookTargetCells), MessageTypeDefOf.NegativeEvent);
        }

        bool ValidRoof(IntVec3 c)
        {
            RoofDef roofDef = roofGrid.RoofAt(c);
            return roofDef != null && !roofDef.isNatural && !roofDef.isThickRoof;
        }
    }
}
