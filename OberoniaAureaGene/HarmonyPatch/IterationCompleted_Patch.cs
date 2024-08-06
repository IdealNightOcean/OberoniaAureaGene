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
        gene?.TryGetBillInspiration();
    }
}
