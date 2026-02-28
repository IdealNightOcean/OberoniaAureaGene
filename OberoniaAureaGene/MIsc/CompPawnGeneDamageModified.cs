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
    protected Pawn ParentPawn => parent as Pawn;

    private List<IGeneIncomingDamageProcessor> GeneProcessors { get; set; }

    public void RegisterGene(Gene_PartIncomingDamageFactor gene)
    {
        if (GeneProcessors is null)
        {
            GeneProcessors = [gene];
            ParentPawn?.GetComp<CompPawnPreApplyDamageHandler>()?.RegisterDamageProcessor(this);
        }
        else if (!GeneProcessors.Contains(gene))
        {
            GeneProcessors.Add(gene);
        }
    }

    public void DeregisterGene(Gene_PartIncomingDamageFactor gene)
    {
        if (GeneProcessors is not null && GeneProcessors.Remove(gene))
        {
            if (GeneProcessors.Count == 0)
            {
                GeneProcessors = null;
                ParentPawn?.GetComp<CompPawnPreApplyDamageHandler>()?.DeregisterDamageProcessor(this);
            }
        }
    }

    public void PawnPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        absorbed = false;
        if (GeneProcessors.NullOrEmpty())
        {
            return;
        }

        if (dinfo.HitPart is null)
        {
            BodyPartDepth bodyPartDepth = (dinfo.Depth == BodyPartDepth.Undefined) ? (Rand.Chance(0.75f) ? BodyPartDepth.Outside : BodyPartDepth.Inside) : dinfo.Depth;
            BodyPartRecord bodyPart = ParentPawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, bodyPartDepth);
            dinfo.SetHitPart(bodyPart);
        }

        foreach (IGeneIncomingDamageProcessor gene in GeneProcessors)
        {
            gene.PreApplyDamage(ref dinfo);
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();

        if (Scribe.mode == LoadSaveMode.PostLoadInit && ParentPawn is not null)
        {
            List<IGeneIncomingDamageProcessor> tmpGeneProcessors = [];
            foreach (Gene gene in ParentPawn.genes.GenesListForReading)
            {
                if (gene is IGeneIncomingDamageProcessor geneProcessor)
                {
                    tmpGeneProcessors.Add(geneProcessor);
                }
            }

            if (tmpGeneProcessors.Count > 0)
            {
                GeneProcessors = tmpGeneProcessors;
            }
        }
    }
}
