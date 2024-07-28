using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene;

public class Gene_PartIncomingDamageFactor : Gene
{
    public GeneExtension geneExtension;
    public GeneExtension GeneExtension => geneExtension ??= def.GetModExtension<GeneExtension>();

    protected List<BodyPartDef> BodyPartDefs => GeneExtension.bodyPartDefs;
    protected float DamageFactor => GeneExtension.damageFactory;
    public float ApplyDamageFactor(BodyPartDef bodyPartDef)
    {
        if (Active && BodyPartDefs.Contains(bodyPartDef))
        {
            return DamageFactor;
        }
        return 1f;
    }
    public override void PostAdd()
    {
        base.PostAdd();
        pawn.GetComp<CompPawnGeneDamageModified>()?.ActivGene(this);
    }
    public override void PostRemove()
    {
        base.PostRemove();
        pawn.GetComp<CompPawnGeneDamageModified>()?.InactivGene(this);
    }
}
