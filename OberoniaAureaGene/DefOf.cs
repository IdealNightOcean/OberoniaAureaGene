using RimWorld;
using Verse;

namespace OberoniaAureaGene;

[DefOf]
public static class OberoniaAureaGeneDefOf
{
    public static HediffDef OAGene_BloodCellsAutophagy;
    public static HediffDef OAGene_DeepSleep;
    public static HediffDef OAGene_SurvivalInstinct;
    public static HediffDef OAGene_XenogermRepairing;

    public static JobDef OAGene_HaluToDiscriminator;

    public static GeneDef OAGene_AbnormalBodyStructure;
    public static GeneDef OAGene_AgriculturalEnthusiasm;
    public static GeneDef OAGene_SpecificHemogen;
    public static GeneDef OAGene_Suspicious;
    public static GeneDef OAGene_BillInspiration;

    static OberoniaAureaGeneDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OberoniaAureaGeneDefOf));
    }
}

[DefOf]
public static class OAGene_RimWorldDefOf
{

    public static HediffDef Frail;
    public static WorkTypeDef Tailoring;

    static OAGene_RimWorldDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_RimWorldDefOf));
    }
}
