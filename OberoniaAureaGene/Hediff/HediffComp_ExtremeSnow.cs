using RimWorld;
using Verse;

namespace OberoniaAureaGene;
public class HediffCompProperties_SnowExtreme : HediffCompProperties
{
    public HediffCompProperties_SnowExtreme()
    {
        compClass = typeof(HediffComp_SnowExtreme);
    }
}
public class HediffComp_SnowExtreme : HediffComp
{
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        TraitSet traitSet = Pawn.story?.traits;
        if (traitSet != null && traitSet.HasTrait(OberoniaAureaGeneDefOf.OAGene_ExtremeSnowSurvivor))
        {
            parent.Severity = 2f;
        }
    }
}
