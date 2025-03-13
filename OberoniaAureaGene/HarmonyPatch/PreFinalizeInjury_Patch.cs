using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(HediffComp_GetsPermanent), "PreFinalizeInjury")]
public static class PreFinalizeInjury_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo geneFactor = AccessTools.Method(typeof(PreFinalizeInjury_Patch), "GeneFactor");
        List<CodeInstruction> codes = [.. instructions];
        for (int i = 0; i < codes.Count; i++)
        {
            CodeInstruction instruction = codes[i];
            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.OperandIs(0.02f))
            {
                yield return instruction;
                yield return new(OpCodes.Ldarg_0);
                yield return new(OpCodes.Ldfld, AccessTools.Field(typeof(HediffComp), "parent"));
                yield return new(OpCodes.Ldfld, AccessTools.Field(typeof(Hediff), "pawn"));
                yield return new(OpCodes.Call, geneFactor);
                instruction = new(OpCodes.Mul);
            }
            yield return instruction;
        }
    }

    public static float GeneFactor(Pawn pawn)
    {
        float geneFactor = 1f;
        if (pawn.genes != null)
        {
            foreach (Gene_PermanentChance gene in pawn.genes.GenesListForReading.OfType<Gene_PermanentChance>())
            {
                geneFactor *= gene.becomePermanentChanceFactor;
            }
        }
        return geneFactor;
    }
}