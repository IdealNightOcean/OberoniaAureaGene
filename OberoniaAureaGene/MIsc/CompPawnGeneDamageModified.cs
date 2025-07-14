using OberoniaAurea_Frame;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace OberoniaAureaGene;

public class CompProperties_PawnGeneDamageModified : CompProperties
{
    public CompProperties_PawnGeneDamageModified()
    {
        compClass = typeof(CompPawnGeneDamageModified);
    }
}
public class CompPawnGeneDamageModified : ThingComp, IPawnPreApplyDamage
{
    public int Priority => 10; // 优先级，数值越大优先级越高

    protected static readonly BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
    protected bool actived = false;
    protected Pawn ParentPawn => parent as Pawn;

    private List<Gene_PartIncomingDamageFactor> activedGenes;

    public void RegisterGene(Gene_PartIncomingDamageFactor gene)
    {
        if (activedGenes is null)
        {
            activedGenes = [gene];
            actived = true;
            ParentPawn?.GetComp<CompPawnPreApplyDamageHandler>()?.RegisterDamageProcessor(this);
        }
        else if (!activedGenes.Contains(gene))
        {
            activedGenes.Add(gene);
        }
    }
    public void DeregisterGene(Gene_PartIncomingDamageFactor gene)
    {
        if (activedGenes is not null && activedGenes.Remove(gene))
        {
            if (activedGenes.Count == 0)
            {
                activedGenes = null;
                actived = false;
                ParentPawn?.GetComp<CompPawnPreApplyDamageHandler>()?.DeregisterDamageProcessor(this);
            }
        }
    }

    public void PawnPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        Log.Message("基因");
        absorbed = false;
        if (!actived)
        {
            return;
        }
        if (dinfo.HitPart is null)
        {
            BodyPartDepth bodyPartDepth = (dinfo.Depth == BodyPartDepth.Undefined) ? (Rand.Chance(0.75f) ? BodyPartDepth.Outside : BodyPartDepth.Inside) : dinfo.Depth;
            BodyPartRecord bodyPart = ParentPawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, bodyPartDepth);
            dinfo.SetHitPart(bodyPart);
        }
        float damageFactor = 1f;
        foreach (Gene_PartIncomingDamageFactor gene in activedGenes)
        {
            damageFactor *= gene.ApplyDamageFactor(dinfo.HitPart.def);
        }
        dinfo.SetAmount(dinfo.Amount * damageFactor);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();

        Scribe_Collections.Look(ref activedGenes, "activedGenes", LookMode.Reference);
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            activedGenes?.RemoveAll(g => g is null);
            if (activedGenes.NullOrEmpty())
            {
                actived = false;
                activedGenes = null;
            }
            else
            {
                actived = true;
                ParentPawn?.GetComp<CompPawnPreApplyDamageHandler>()?.RegisterDamageProcessor(this);
            }
        }
    }
}
