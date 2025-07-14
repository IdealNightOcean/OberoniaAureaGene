using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(CompBiosculpterPod), "CannotUseNowPawnReason")]
public static class CompBiosculpterPod_Patch
{
    [HarmonyPostfix]
    public static void CannotUseNowPawnReason_Postfix(ref string __result, Pawn p)
    {
        if (__result is null)
        {
            if (p.genes is not null && p.genes.HasActiveGene(OAGene_GeneDefOf.OAGene_AbnormalBodyStructure))
            {
                __result = "OAGene_AbnormalBodyStructure".Translate();
                return;
            }
        }
    }
}
