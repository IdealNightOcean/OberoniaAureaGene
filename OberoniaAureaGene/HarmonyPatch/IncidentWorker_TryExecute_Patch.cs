using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(IncidentWorker), "TryExecute")]

public static class TryExecute_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref IncidentWorker __instance, ref bool __result, IncidentParms parms)
    {
        if (!ModsConfig.IdeologyActive || !__result)
        {
            return;
        }
        if (__instance.def.category != IncidentCategoryDefOf.ThreatBig)
        {
            return;
        }
        if (parms.target is Map map)
        {
            Notify_ThreatBigEvent(map);
        }
    }

    private static void Notify_ThreatBigEvent(Map map)
    {
        map.GetOAGeneMapComp()?.QuickEnemyCheck(25);
        IEnumerable<Pawn> aliveColonists = map.mapPawns.FreeColonists.Where(p => !p.Dead);
        foreach (Pawn pawn in aliveColonists)
        {
            HistoryEvent historyEvent = new(OberoniaAureaGeneDefOf.OAGene_ThreatBig, pawn.Named(HistoryEventArgsNames.Doer));
            Find.HistoryEventsManager.RecordEvent(historyEvent);
        }
        Find.HistoryEventsManager.RecordEvent(new HistoryEvent(OberoniaAureaGeneDefOf.OAGene_PlayerThreatBig));
    }
}
