using RimWorld;
using Verse;

namespace OberoniaAureaGene;

//暴风雪幸存者 的 极端暴风雪Hediff 进入不同的严重度阶段
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
        TraitSet traitSet = parent.pawn.story?.traits;
        if (traitSet is not null && traitSet.HasTrait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor))
        {
            parent.Severity = 2f;
        }
    }
}
