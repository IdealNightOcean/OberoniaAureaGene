using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;


[StaticConstructorOnStartup]
[HarmonyPatch(typeof(PregnancyUtility), "GetInheritedGeneSet",
    [typeof(Pawn), typeof(Pawn), typeof(bool)],
    [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out])]

public static class GetInheritedGeneSet_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref GeneSet __result, Pawn father, Pawn mother)
    {
        if (mother.IsRatkin())
        {
            __result.AddGene(OAGene_RatkinGeneDefOf.OAGene_RatkinEar);
            __result.AddGene(OAGene_RatkinGeneDefOf.OAGene_RatkinTail);
        }
    }
}
