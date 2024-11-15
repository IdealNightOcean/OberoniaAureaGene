using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_IceStorm : HediffCompProperties
{
    public FloatRange damageAmount;
    public FloatRange damageArmorPenetration;

    public IntRange damageInterval;

    public HediffCompProperties_IceStorm()
    {
        compClass = typeof(HediffComp_IceStorm);
    }
}
public class HediffComp_IceStorm : HediffComp
{
    HediffCompProperties_IceStorm Props => props as HediffCompProperties_IceStorm;

    protected int ticksRemaining;

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            DamageInfo dinfo = new(Snowstrom_MiscDefOf.OAGene_IceStab, Props.damageAmount.RandomInRange, Props.damageArmorPenetration.RandomInRange);
            parent.pawn.TakeDamage(dinfo);
            ticksRemaining = Props.damageInterval.RandomInRange;
        }
    }
    public override void CompExposeData()
    {
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
    }
}
