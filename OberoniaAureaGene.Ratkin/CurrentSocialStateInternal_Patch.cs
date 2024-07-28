using AlienRace;
using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;
/*
[StaticConstructorOnStartup]
[HarmonyPatch]
public static class CurrentSocialStateInternal_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ThoughtWorker_AlienVsXenophobia), "CurrentSocialStateInternal")]
    public static void AlienVsXenophobia_Patch(ref ThoughtState __result, Pawn p, Pawn otherPawn)
    {
        if (!__result.Active)
        {
            return;
        }
        if (p.genes == null || otherPawn.genes == null)
        {
            return;
        }
        Gene_RatkinEar gene_RatkinEar1 = p.genes.GetFirstGeneOfType<Gene_RatkinEar>();
        if (gene_RatkinEar1 == null || !gene_RatkinEar1.actived)
        {
            return;
        }
        Gene_RatkinEar gene_RatkinEar2 = otherPawn.genes.GetFirstGeneOfType<Gene_RatkinEar>();
        if (gene_RatkinEar2 == null || !gene_RatkinEar2.actived)
        {
            return;
        }
        __result = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ThoughtWorker_XenophobiaVsAlien), "CurrentSocialStateInternal")]
    public static void XenophobiaVsAlien_Patch(ref ThoughtState __result, Pawn p, Pawn otherPawn)
    {
        if (!__result.Active)
        {
            return;
        }
        if (p.genes == null || otherPawn.genes == null)
        {
            return;
        }
        Gene_RatkinEar gene_RatkinEar1 = p.genes.GetFirstGeneOfType<Gene_RatkinEar>();
        if (gene_RatkinEar1 == null || !gene_RatkinEar1.actived)
        {
            return;
        }
        Gene_RatkinEar gene_RatkinEar2 = otherPawn.genes.GetFirstGeneOfType<Gene_RatkinEar>();
        if (gene_RatkinEar2 == null || !gene_RatkinEar2.actived)
        {
            return;
        }
        __result = false;
    }
}
*/