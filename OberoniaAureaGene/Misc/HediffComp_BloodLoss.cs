using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class HediffCompProperties_BloodLoss : HediffCompProperties
{
    public float bleedRatePreTick;
    public HediffCompProperties_BloodLoss()
    {
        compClass = typeof(HediffComp_BloodLoss);
    }
}

public class HediffComp_BloodLoss : HediffComp
{
    public HediffCompProperties_BloodLoss Props => props as HediffCompProperties_BloodLoss;
    public float BleedRateRareTick => Props.bleedRatePreTick * 250f;
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (Pawn.IsHashIntervalTick(250))
        {
            TryAdjuestBleed(Pawn, BleedRateRareTick);
        }
    }
    protected static void TryAdjuestBleed(Pawn pawn, float bleedRate)
    {
        pawn.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out var bloodLoss);
        if (bloodLoss is null)
        {
            bloodLoss = pawn.health.AddHediff(HediffDefOf.BloodLoss);
            bloodLoss.Severity = 0.01f;
        }
        bloodLoss.Severity += bleedRate;
    }
}
