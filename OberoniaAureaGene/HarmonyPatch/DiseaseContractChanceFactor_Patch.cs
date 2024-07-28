using HarmonyLib;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(ImmunityHandler), "DiseaseContractChanceFactor",
    [typeof(HediffDef), typeof(HediffDef), typeof(BodyPartRecord)],
    [ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal])]
public static class DiseaseContractChanceFactor_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref ImmunityHandler __instance, ref float __result)
    {
        if (__result == 0f)
        {
            return;
        }
        Pawn pawn = __instance.pawn;
        if (pawn.genes == null)
        {
            return;
        }
        float factor = 1f;
        foreach (Gene_IllnessFactor gene in pawn.genes.GenesListForReading.OfType<Gene_IllnessFactor>())
        {
            factor *= gene.illnessFactor;
        }
        __result *= factor;
    }

}
