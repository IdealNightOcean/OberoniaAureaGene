using RimWorld;
using Verse;

namespace OberoniaAureaGene
{
    public class HediffCompProperties_ExperienceSnowstorm : HediffCompProperties
    {
        public HediffCompProperties_ExperienceSnowstorm()
        {
            compClass = typeof(HeidffComp_ExperienceSnowstorm);
        }
    }

    public class HeidffComp_ExperienceSnowstorm : HediffComp
    {
        public int experienceCount;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if (parent.pawn.story?.traits?.HasTrait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor) ?? true)
            {
                parent.pawn.health.RemoveHediff(parent);
            }
            Notify_ExperienceSnowstorm(parent.pawn);
        }

        public void Notify_ExperienceSnowstorm(Pawn pawn)
        {
            experienceCount++;
            if (experienceCount >= 5)
            {
                pawn.story?.traits?.GainTrait(new Trait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor));
            }
        }
    }
}
