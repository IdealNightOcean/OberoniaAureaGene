using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(CompStatue), "InitFakePawn_HookForMods")]
public static class InitFakePawn_HookForMods_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Pawn fakePawn, Dictionary<string, object> additionalSavedPawnDataForMods)
    {
        if (fakePawn.IsRatkin())
        {
            List<Gene> endogenes = fakePawn.genes.Endogenes;
            for (int i = endogenes.Count - 1; i >= 0; i--)
            {
                GeneDef addGene = endogenes[i].def;
                fakePawn.genes.RemoveGene(endogenes[i]);
                fakePawn.genes.AddGene(addGene, xenogene: true);
            }
        }
    }
}