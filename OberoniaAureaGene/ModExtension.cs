using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene;

public class WorkGiverHaulToExtension : DefModExtension
{
    public ThingDef thingDef;

}

public class GeneExtension : DefModExtension
{
    public HediffDef hediffToWholeBody;
    public float becomePermanentChanceFactor = 1f;

    public List<BodyPartDef> bodyPartDefs;
    public float damageFactory = 1f;
    public float illnessFactor = 1f;
}