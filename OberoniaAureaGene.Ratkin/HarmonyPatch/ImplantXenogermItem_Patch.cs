using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.ImplantXenogermItem))]

public static class ImplantXenogermItem_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(Pawn pawn, Xenogerm xenogerm)
    {
        if (pawn.IsRatkin())
        {
            RatkinImplantXenogermItem(pawn, xenogerm);
            return false;
        }
        return true;
    }

    private static void RatkinImplantXenogermItem(Pawn pawn, Xenogerm xenogerm)
    {
        GeneUtility.UpdateXenogermReplication(pawn);
        if (xenogerm.GeneSet is null || pawn.genes is null)
        {
            return;
        }
        pawn.genes.SetXenotype(OAGene_RatkinDefOf.OAGene_RatkinBase);
        pawn.genes.xenotypeName = xenogerm.xenotypeName;
        pawn.genes.iconDef = xenogerm.iconDef;
        foreach (GeneDef item in xenogerm.GeneSet.GenesListForReading)
        {
            pawn.genes.AddGene(item, xenogene: true);
        }
    }
}
