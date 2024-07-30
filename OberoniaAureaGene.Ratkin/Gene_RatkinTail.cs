namespace OberoniaAureaGene.Ratkin;

public class Gene_RatkinTail : Gene_GiveHediff
{
    public override void PostAdd()
    {
        if (pawn.def != OAGene_RatkinDefOf.Ratkin && pawn.def != OAGene_RatkinDefOf.Ratkin_Su)
        {
            pawn.genes.RemoveGene(this);
            return;
        }
        base.PostAdd();
    }
}
