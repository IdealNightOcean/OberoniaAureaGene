using Verse;

namespace OberoniaAureaGene.Ratkin;

public class Gene_RatkinEar : Gene
{
    public bool actived;

    public override void PostAdd()
    {
        base.PostAdd();
        RecachedGene();
    }
    public override void PostRemove()
    {
        base.PostRemove();
        RecachedGene();
    }
    public void RecachedGene()
    {
        actived = RecachedGene(pawn);
    }
    private static bool RecachedGene(Pawn pawn)
    {
        if (!pawn.genes.HasGene(OAGene_RatkinDefOf.OAGene_RatkinTail))
        {
            return false;
        }
        if (!pawn.genes.HasGene(OAGene_RatkinDefOf.OAGene_RatkinBody))
        {
            return false;
        }
        return true;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref actived, "actived", defaultValue: false);
    }
}
