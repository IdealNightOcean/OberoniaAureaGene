using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(NegativeInteractionUtility), "NegativeInteractionChanceFactor")]
public static class NegativeInteractionChanceFactor_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref float __result, Pawn initiator)
    {
        if (__result == 0f)
        {
            return;
        }
        if (initiator.health.hediffSet.HasHediff(Snowstorm_HediffDefOf.OAGene_Hediff_SnowstormAngry))
        {
            __result *= 5f;
        }
    }
}
