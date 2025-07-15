using Verse;

namespace OberoniaAureaGene.Ratkin;

public class Gene_RockRatkinEar : Gene_PartIncomingDamageFactor
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
        if (hediff is not null)
        {
            pawn.health.RemoveHediff(hediff);
        }
    }
}
