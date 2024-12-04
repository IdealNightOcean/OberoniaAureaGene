using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_FixedSnowstormBreachingRaids : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;

    public SlateRef<float> currentThreatPointsFactor = 1f;
    public SlateRef<float> minThreatPoints = -1f;
    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }
    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        MapParent hometown = slate.Get<WorldObject>("hometown") as MapParent;
        if (hometown == null)
        {
            return;
        }
        IncidentParms parms = new()
        {
            forced = true,
        };
        QuestPart_EndGame_Incident questPart_EndGame_Incident = new()
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            incident = Snowstrom_IncidentDefOf.OAGene_SnowstormMaliceRaid,
            currentThreatPointsFactor = currentThreatPointsFactor.GetValue(slate),
            minThreatPoints = minThreatPoints.GetValue(slate),
        };
        questPart_EndGame_Incident.SetIncidentParmsAndRemoveTarget(parms, hometown);
        QuestGen.quest.AddPart(questPart_EndGame_Incident);
    }

}
