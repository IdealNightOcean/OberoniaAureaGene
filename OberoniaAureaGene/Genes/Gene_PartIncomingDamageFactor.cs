using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene;




public class Gene_PartIncomingDamageFactor : Gene, IGeneIncomingDamageProcessor
{
    public GeneExtension geneExtension;
    public GeneExtension GeneExtension => geneExtension ??= def.GetModExtension<GeneExtension>();

    protected List<BodyPartDef> BodyPartDefs => GeneExtension.bodyPartDefs;
    protected float DamageFactor => GeneExtension.damageFactory;

    public void PreApplyDamage(ref DamageInfo dinfo)
    {
        if (Active && BodyPartDefs.Contains(dinfo.HitPart.def))
        {
            dinfo.SetAmount(dinfo.Amount * DamageFactor);
        }
    }

    public override void PostAdd()
    {
        base.PostAdd();
        pawn.GetComp<CompPawnGeneDamageModified>()?.RegisterGene(this);
    }
    public override void PostRemove()
    {
        base.PostRemove();
        pawn.GetComp<CompPawnGeneDamageModified>()?.DeregisterGene(this);
    }
}
