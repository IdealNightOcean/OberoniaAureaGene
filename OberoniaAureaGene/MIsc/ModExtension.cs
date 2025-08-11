using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene;

public class GeneExtension : DefModExtension
{
    public HediffDef hediffToWholeBody;
    public float becomePermanentChanceFactor = 1f;

    public List<BodyPartDef> bodyPartDefs;
    public float damageFactory = 1f;
    public float illnessFactor = 1f;
}

public class ObeyOrderGeneExtension : DefModExtension
{
    public HediffDef hediffToWholeBody;

    public HediffDef hediffWorking;
    public HediffDef hediffNonDraft;
}