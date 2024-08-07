namespace OberoniaAureaGene.Ratkin;

public class Gene_RatkinTail : Gene_GiveHediff
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
