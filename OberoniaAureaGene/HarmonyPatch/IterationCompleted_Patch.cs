using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Bill_Production), "Notify_IterationCompleted")]
public static class IterationCompleted_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref Bill_Production __instance, Pawn billDoer)
    {
        WorkTypeDef workType = __instance.billStack.billGiver.GetWorkgiver()?.workType;
        if (workType == WorkTypeDefOf.Smithing || workType == OAGene_RimWorldDefOf.Tailoring)
        {
            TryGetBillInspiration(billDoer);
        }
    }
    private static void TryGetBillInspiration(Pawn pawn)
    {
        if (pawn.genes == null)
        {
            return;
        }
        Gene_BillInspiration gene = (Gene_BillInspiration)pawn.genes.GetGene(OberoniaAureaGeneDefOf.OAGene_BillInspiration);
        if (gene == null || gene.Cooling)
        {
            return;
        }
        if (!InspirationDefOf.Inspired_Creativity.Worker.InspirationCanOccur(pawn))
        {
            return;
        }
        if (pawn.mindState.inspirationHandler.Inspired)
        {
            return;
        }
        float chance = 0.01f;
        Need_Mood pawnMood = pawn.needs.mood;
        if (pawnMood != null)
        {
            float validPercentage = (pawnMood.CurInstantLevelPercentage - 0.5f) * 0.1f;
            chance += validPercentage > 0f ? validPercentage : 0f;
        }
        if (Rand.Chance(chance))
        {
            pawn.mindState.inspirationHandler.TryStartInspiration(InspirationDefOf.Inspired_Creativity, "OAGene_LetterBillInspiration".Translate(pawn.Named("PAWN")));
            gene.Notify_SuccessfullyInspired();
        }

    }
}
