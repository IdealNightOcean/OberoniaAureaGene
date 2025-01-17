﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

//特异血原输血
[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Recipe_BloodTransfusion), "AvailableOnNow")]
public static class BloodTransfusion_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, Thing thing)
    {
        if (__result && thing is Pawn pawn && pawn.genes != null)
        {
            if (pawn.genes.HasActiveGene(OAGene_GeneDefOf.OAGene_SpecificHemogen))
            {
                __result = false;
            }
        }
    }
}