using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

//特异血原相关
[StaticConstructorOnStartup]
[HarmonyPatch(typeof(CompAbilityEffect_BloodfeederBite), "Valid")]
public static class BloodfeederBite_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, LocalTargetInfo target)
    {
        if (__result)
        {
            Pawn pawn = target.Pawn;
            if (pawn.genes != null && pawn.genes.HasGene(OberoniaAureaGeneDefOf.OAGene_SpecificHemogen))
            {
                __result = false;
            }
        }
    }
}
