using Verse;

namespace OberoniaAureaGene;

public class Gene_GiveHediff : Gene
{
    public HediffDef LinkedHediffDef => def.GetModExtension<GeneExtension>().hediffToWholeBody;

    public override void PostAdd()
    {
        base.PostAdd();
        pawn.health.AddHediff(LinkedHediffDef);
    }
    public override void PostRemove()
    {
        base.PostRemove();
        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(LinkedHediffDef);
        if (hediff != null)
        {
            pawn.health.RemoveHediff(hediff);
        }
    }
}


