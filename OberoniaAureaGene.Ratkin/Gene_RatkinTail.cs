using Verse;

namespace OberoniaAureaGene.Ratkin;

public class Gene_RatkinTail : Gene_GiveHediff
{
    public override void PostAdd()
    {
        base.PostAdd();
        RecachedGene(pawn);
    }
    public override void PostRemove()
    {
        base.PostRemove();
        RecachedGene(pawn);
    }
    public static void RecachedGene(Pawn pawn)
    {
        Gene_RatkinEar gene_RatkinEar = pawn.genes.GetFirstGeneOfType<Gene_RatkinEar>();
        gene_RatkinEar?.RecachedGene();
    }
}
