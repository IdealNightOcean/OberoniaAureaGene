using Verse;

namespace OberoniaAureaGene.Ratkin;

public class Gene_RatkinBase : Gene
{
    public override void PostAdd()
    {
        if (!pawn.IsRatkin())
        {
            pawn.genes.RemoveGene(this);
            return;
        }
        base.PostAdd();
    }
}
