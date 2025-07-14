using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(ThoughtHandler), "OpinionOffsetOfGroup")]
public static class OpinionOffsetOfGroup_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref ThoughtHandler __instance, ref int __result)
    {
        Pawn pawn = __instance.pawn;
        if (pawn.genes is not null && pawn.genes.HasActiveGene(OAGene_GeneDefOf.OAGene_Suspicious))
        {
            __result /= 2;
        }
    }
}
