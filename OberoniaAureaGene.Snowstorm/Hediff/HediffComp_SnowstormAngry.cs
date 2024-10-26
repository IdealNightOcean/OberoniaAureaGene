using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_SnowstormAngry : HediffCompProperties
{
    public ThoughtDef stormSleepAngryThought;

    public HediffCompProperties_SnowstormAngry()
    {
        compClass = typeof(HediffComp_SnowstormAngry);
    }

}

public class HediffComp_SnowstormAngry : HediffComp
{
    HediffCompProperties_SnowstormAngry Props => props as HediffCompProperties_SnowstormAngry;
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn.IsHashIntervalTick(1000))
        {
            Pawn pawn = parent.pawn;
            if (!OAFrame_PawnUtility.PawnSleepNow(pawn))
            {
                return;
            }
            Building bed = pawn.ownership.OwnedBed;
            if (bed == null || bed.GetRoom()?.Role != RoomRoleDefOf.Barracks)
            {
                return;
            }
            pawn.needs.mood?.thoughts.memories.TryGainMemory(Props.stormSleepAngryThought);
        }
    }


}
