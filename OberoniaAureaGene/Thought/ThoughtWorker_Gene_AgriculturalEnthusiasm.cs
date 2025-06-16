using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_Gene_AgriculturalEnthusiasm : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p.genes is null)
        {
            return ThoughtState.Inactive;
        }
        if (p.genes.GetGene(OAGene_GeneDefOf.OAGene_AgriculturalEnthusiasm) is Gene_AgriculturalEnthusiasm gene)
        {
            if (gene.activeThought)
            {
                return ThoughtState.ActiveDefault;
            }
        }
        return ThoughtState.Inactive;
    }
}
