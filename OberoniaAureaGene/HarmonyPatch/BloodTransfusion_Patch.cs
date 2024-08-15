﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

//特异血原相关
[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Recipe_BloodTransfusion), "AvailableOnNow")]
public static class BloodTransfusion_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, Thing thing)
    {
        if (__result && thing is Pawn pawn && pawn.genes != null)
        {
            if (pawn.genes.HasActiveGene(OberoniaAureaGeneDefOf.OAGene_SpecificHemogen))
            {
                __result = false;
            }
        }
    }
}