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
    private HediffCompProperties_SnowstormAngry Props => props as HediffCompProperties_SnowstormAngry;

    public override string CompTipStringExtra => "OAGene_SnowstormAngry_NegativeInteractionFactor".Translate(5f.ToStringPercent());
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn.IsHashIntervalTick(1000))
        {
            Pawn pawn = parent.pawn;
            if (!pawn.Spawned || !pawn.SleepNow())
            {
                return;
            }
            Building bed = pawn.ownership.OwnedBed;
            if (bed is null || bed.GetRoom()?.Role != RoomRoleDefOf.Barracks)
            {
                return;
            }
            pawn.needs.mood?.thoughts.memories.TryGainMemory(Props.stormSleepAngryThought);
        }
    }


}
