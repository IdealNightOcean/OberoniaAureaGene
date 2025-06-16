using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

//特异血原提取血原质
[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Recipe_ExtractHemogen), "AvailableOnNow")]
public static class ExtractHemogen_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, Thing thing)
    {
        if (__result && thing is Pawn pawn && pawn.genes is not null)
        {
            if (pawn.genes.HasActiveGene(OAGene_GeneDefOf.OAGene_SpecificHemogen))
            {
                __result = false;
            }
        }
    }
}