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
    protected Pawn parentPawn;

    private List<Gene_PartIncomingDamageFactor> activedGenes;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
    }
    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (parent is Pawn pawn)
        {
            parentPawn = pawn;
            pawn.GetComp<CompPawnPreApplyDamageHandler>()?.RegisterDamageProcessor(this);
        }
    }

    public void RegisterGene(Gene_PartIncomingDamageFactor gene)
    {
        if (activedGenes is null)
        {
            activedGenes = [gene];
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
            }
        }
    }

    public void PawnPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        absorbed = false;
        if (dinfo.Def.Worker is not DamageWorker_AddInjury addInjury)
        {
            return;
        }
        if (dinfo.HitPart == null)
        {
            BodyPartDepth bodyPartDepth = (dinfo.Depth == BodyPartDepth.Undefined) ? (Rand.Chance(0.75f) ? BodyPartDepth.Outside : BodyPartDepth.Inside) : dinfo.Depth;
            BodyPartRecord bodyPart = parentPawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, bodyPartDepth);
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
        Scribe_Values.Look(ref actived, "actived", defaultValue: false);
        Scribe_Collections.Look(ref activedGenes, "activedGenes", LookMode.Reference);
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            activedGenes.RemoveAll(g => g is null);
        }
    }
}
