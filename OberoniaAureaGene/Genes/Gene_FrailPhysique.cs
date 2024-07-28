using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class Gene_FrailPhysique : Gene
{
    private int ticksRemaining;
    private bool actived;

    public override void PostAdd()
    {
        base.PostAdd();
        TryAddHediff();
    }

    public override void Tick()
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            if (!actived)
            {
                TryAddHediff();
            }
            ticksRemaining = 60000;
        }
    }

    private void TryAddHediff()
    {
        float pawnLifeExpectancy = pawn.GetStatValue(StatDefOf.LifespanFactor) * pawn.RaceProps.lifeExpectancy;
        if (pawn.ageTracker.AgeBiologicalYears > pawnLifeExpectancy * 0.5f)
        {
            pawn.health.AddHediff(OAGene_RimWorldDefOf.Frail);
            actived = true;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref actived, "actived", defaultValue: false);
    }
}
