using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_FixedMechBreachingRaids : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;
    [NoTranslate]
    public SlateRef<string> inSignalDisable;

    public SlateRef<int> delayTicks;

    public SlateRef<IncidentDef> incidentDef;
    public SlateRef<float> currentThreatPointsFactor = 1f;
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
            faction = Faction.OfMechanoids,
            raidStrategy = Snowstrom_RimWorldDefOf.ImmediateAttackBreaching,
        };
        QuestPart_EndGame_Incident questPart_EndGame_Incident = new()
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            incident = IncidentDefOf.RaidEnemy,
            currentThreatPointsFactor = currentThreatPointsFactor.GetValue(slate),
        };
        questPart_EndGame_Incident.SetIncidentParmsAndRemoveTarget(parms, hometown);
        QuestGen.quest.AddPart(questPart_EndGame_Incident);
    }
}
