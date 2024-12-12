using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_ExperienceSnowstorm : HediffCompProperties
{
    public HediffCompProperties_ExperienceSnowstorm()
    {
        compClass = typeof(HediffComp_ExperienceSnowstorm);
    }
}

public class HediffComp_ExperienceSnowstorm : HediffComp
{
    public int experienceCount;

    public override string CompLabelInBracketsExtra => experienceCount.ToString();
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        if (parent.pawn.story?.traits?.HasTrait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor) ?? true)
        {
            parent.pawn.health.RemoveHediff(parent);
            return;
        }
        Notify_ExperienceSnowstorm(parent.pawn);
    }

    public override void CompPostMerged(Hediff other)
    {
        Notify_ExperienceSnowstorm(parent.pawn);
    }
    public void Notify_ExperienceSnowstorm(Pawn pawn)
    {
        experienceCount++;
        if (experienceCount >= 5)
        {
            pawn.story?.traits?.GainTrait(new Trait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor, 0), true);
            parent.pawn.health.RemoveHediff(parent);
        }
    }
    public override void CompExposeData()
    {
        Scribe_Values.Look(ref experienceCount, "experienceCount", 0);
    }
}

