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
public class CompPawnGeneDamageModified : ThingComp
{
    protected static readonly BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
    protected bool actived = false;
    protected Pawn pawn;

    private List<Gene_PartIncomingDamageFactor> activedGenes;
    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        pawn = parent as Pawn;
    }
    public void ActivGene(Gene_PartIncomingDamageFactor gene)
    {
        activedGenes ??= [];
        if (!activedGenes.Contains(gene))
        {
            activedGenes.Add(gene);
        }
        RecacheActivedGenes();
    }
    public void InactivGene(Gene_PartIncomingDamageFactor gene)
    {
        activedGenes?.Remove(gene);
        RecacheActivedGenes();
    }
    private void RecacheActivedGenes()
    {
        activedGenes?.RemoveAll(g => g == null);
        if (activedGenes.NullOrEmpty())
        {
            activedGenes = null;
            actived = false;
        }
        else
        {
            actived = true;
        }
    }

    public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        absorbed = false;
        if (!actived)
        {
            return;
        }
        if (dinfo.Def.Worker is not DamageWorker_AddInjury addInjury)
        {
            return;
        }
        if (dinfo.HitPart == null)
        {
            BodyPartRecord bodyPart = ChooseHitPart(addInjury, dinfo, pawn);
            dinfo.SetHitPart(bodyPart);
        }
        float damageFactor = 1f;
        foreach (Gene_PartIncomingDamageFactor gene in activedGenes)
        {
            damageFactor *= gene.ApplyDamageFactor(dinfo.HitPart.def);
        }
        dinfo.SetAmount(dinfo.Amount * damageFactor);
    }
    protected static BodyPartRecord ChooseHitPart(DamageWorker_AddInjury addInjury, DamageInfo dinfo, Pawn pawn)
    {
        object obj = addInjury.GetType().GetMethod("ChooseHitPart", BindingAttr).Invoke(addInjury, [dinfo, pawn]);
        return obj as BodyPartRecord;
    }
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref actived, "actived", defaultValue: false);
        Scribe_Collections.Look(ref activedGenes, "activedGenes", LookMode.Reference);
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            RecacheActivedGenes();
        }
    }
}
