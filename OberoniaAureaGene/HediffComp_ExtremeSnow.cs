using RimWorld;
using Verse;

namespace OberoniaAureaGene;
public class HediffCompProperties_ExtremeSnow : HediffCompProperties
{
    public HediffCompProperties_ExtremeSnow()
    {
        compClass = typeof(HediffComp_ExtremeSnow);
    }
}
public class HediffComp_ExtremeSnow : HediffComp
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
