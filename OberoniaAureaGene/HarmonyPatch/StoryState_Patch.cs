using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(StoryState), "Notify_IncidentFired")]
public static class StoryState_Patch
{
    [HarmonyPostfix]
    public static void Postfix(FiringIncident fi)
    {
        if (!ModsConfig.IdeologyActive)
        {
            return;
        }
        if (fi.def.category != IncidentCategoryDefOf.ThreatBig)
        {
            return;
        }
        if (fi.parms.target is Map map)
        {
            Notify_ThreatBigEvent(map);
        }
    }
    private static void Notify_ThreatBigEvent(Map map)
    {
        var aliveColonists = map.mapPawns.FreeColonists.Where(p => !p.Dead);
        foreach (Pawn pawn in aliveColonists)
        {
            HistoryEvent historyEvent = new(OberoniaAureaGeneDefOf.OAGene_ThreatBig, pawn.Named(HistoryEventArgsNames.Doer));
            Find.HistoryEventsManager.RecordEvent(historyEvent);
        }
        Find.HistoryEventsManager.RecordEvent(new HistoryEvent(OberoniaAureaGeneDefOf.OAGene_PlayerThreatBig));
    }
}
