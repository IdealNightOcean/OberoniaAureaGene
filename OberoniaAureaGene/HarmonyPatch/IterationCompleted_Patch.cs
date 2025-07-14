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
        if (billDoer is null || billDoer.genes is null)
        {
            return;
        }
        WorkTypeDef workType = __instance.billStack?.billGiver?.GetWorkgiver()?.workType;
        if (workType == WorkTypeDefOf.Smithing || workType == OAGene_RimWorldDefOf.Tailoring)
        {
            Gene_BillInspiration gene = (Gene_BillInspiration)billDoer.genes.GetGene(OAGene_GeneDefOf.OAGene_BillInspiration);
            gene?.TryGetBillInspiration();
        }
    }
}
