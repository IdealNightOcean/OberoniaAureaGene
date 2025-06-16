using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class CompProperties_GeneDiscriminat : CompProperties_GenepackContainer
{
    public CompProperties_GeneDiscriminat()
    {
        compClass = typeof(CompGeneDiscriminat);
    }
}
public class CompGeneDiscriminat : CompGenepackContainer
{
    public override void PostPostMake()
    {
        base.PostPostMake();
        autoLoad = false;
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        return Enumerable.Empty<Gizmo>();
    }

    public override string CompInspectStringExtra()
    {
        return string.Empty;
    }
}
