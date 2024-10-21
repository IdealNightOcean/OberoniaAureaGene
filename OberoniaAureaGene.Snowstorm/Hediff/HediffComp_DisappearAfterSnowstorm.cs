using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_DisappearAfterSnowstorm : HediffCompProperties
{
    public IntRange checkInterval;

    public HediffCompProperties_DisappearAfterSnowstorm()
    {
        compClass = typeof(HediffComp_DisappearAfterSnowstorm);
    }
}

public class HediffComp_DisappearAfterSnowstorm : HediffComp
{
    public HediffCompProperties_DisappearAfterSnowstorm Props => props as HediffCompProperties_DisappearAfterSnowstorm;

    protected int ticksRemaining = new IntRange(500, 600).RandomInRange;

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            if (SnowstormUtility.IsIceStormWeather(parent.pawn.Map))
            {
                parent.pawn.health.RemoveHediff(parent);
                return;
            }
            ticksRemaining = Props.checkInterval.RandomInRange;
        }
    }
}
