using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_UnderDark : HediffCompProperties
{
    public float mtbHours;
    public float threshPercent;
    public HediffCompProperties_UnderDark()
    {
        compClass = typeof(HediffComp_UnderDark);
    }
}

public class HediffComp_UnderDark : HediffComp
{
    HediffCompProperties_UnderDark Props => props as HediffCompProperties_UnderDark;
    public override void CompPostTick(ref float severityAdjustment)
    {
        Pawn parentPawn = parent.pawn;
        if (parentPawn.IsHashIntervalTick(250))
        {
            Need_SnowstormGlow need_SnowstormGlow = parentPawn.needs?.TryGetNeed<Need_SnowstormGlow>();
            if (need_SnowstormGlow == null)
            {
                parentPawn.health.RemoveHediff(parent);
                return;
            }
            if (need_SnowstormGlow.CurLevel > Props.threshPercent)
            {
                return;
            }
            if (Rand.Value < 250f / (Props.mtbHours * 2500f))
            {
                parentPawn.mindState.mentalStateHandler.Reset();
                parentPawn.mindState.mentalBreaker.TryDoMentalBreak("OAGene_UnderDarkMentalBreak", OAGene_RimWorldDefOf.Wander_Psychotic);
            }
        }
    }
}
