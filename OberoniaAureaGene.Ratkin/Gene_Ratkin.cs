using Verse;

namespace OberoniaAureaGene.Ratkin;

public class Gene_RatkinBase : Gene
{
    public override void PostAdd()
    {
        base.PostAdd();
        if (pawn.def != OAGene_RatkinDefOf.Ratkin && pawn.def != OAGene_RatkinDefOf.Ratkin_Su)
        {
            pawn.genes.RemoveGene(this);
        }
    }
}
