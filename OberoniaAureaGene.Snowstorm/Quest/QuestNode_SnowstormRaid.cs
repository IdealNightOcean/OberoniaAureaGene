using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_SnowstormRaid : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;
    public SlateRef<IntVec3?> walkInSpot;
    public SlateRef<bool?> canTimeoutOrFlee;
    [NoTranslate]
    public SlateRef<string> inSignalLeave;
    [NoTranslate]
    public SlateRef<string> tag;

    protected override bool TestRunInt(Slate slate)
    {
        if (!Find.Storyteller.difficulty.allowViolentQuests)
        {
            return false;
        }
        if (!slate.Exists("map"))
        {
            return false;
        }
        if (!slate.Exists("enemyFaction"))
        {
            return false;
        }
        return true;
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        Map map = QuestGen.slate.Get<Map>("map");
        float points = QuestGen.slate.Get("points", 0f);
        Faction faction = QuestGen.slate.Get<Faction>("enemyFaction");
        QuestPart_Incident questPart_Incident = new QuestPart_Incident
        {
            debugLabel = "raid",
            incident = IncidentDefOf.RaidEnemy
        };
        int num = 0;
        IncidentParms incidentParms;
        PawnGroupMakerParms defaultPawnGroupMakerParms;
        IEnumerable<PawnKindDef> enumerable;
        do
        {
            incidentParms = GenerateIncidentParms(map, points, faction, slate, questPart_Incident);
            defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms, ensureCanGenerateAtLeastOnePawn: true);
            defaultPawnGroupMakerParms.points = IncidentWorker_Raid.AdjustedRaidPoints(points: defaultPawnGroupMakerParms.points,
                                                                                       raidArrivalMode: incidentParms.raidArrivalMode,
                                                                                       raidStrategy: incidentParms.raidStrategy,
                                                                                       faction: defaultPawnGroupMakerParms.faction,
                                                                                       groupKind: PawnGroupKindDefOf.Combat,
                                                                                       target: map);

            enumerable = PawnGroupMakerUtility.GeneratePawnKindsExample(defaultPawnGroupMakerParms);
            num++;
        }
        while (!enumerable.Any() && num < 50);
        if (!enumerable.Any())
        {
            Log.Error(string.Concat("[OAGene] No pawnkinds example for ", QuestGen.quest.root.defName, " parms=", defaultPawnGroupMakerParms, " iterations=", num));
        }
        questPart_Incident.SetIncidentParmsAndRemoveTarget(incidentParms);
        questPart_Incident.inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? QuestGen.slate.Get<string>("inSignal");
        QuestGen.quest.AddPart(questPart_Incident);
        QuestGen.AddQuestDescriptionRules(
        [
            new Rule_String("raidPawnKinds", PawnUtility.PawnKindsToLineList(enumerable, "  - ", ColoredText.ThreatColor)),
            new Rule_String("raidArrivalModeInfo", incidentParms.raidArrivalMode.textWillArrive.Formatted(faction))
        ]);
    }

    private IncidentParms GenerateIncidentParms(Map map, float points, Faction faction, Slate slate, QuestPart_Incident questPart)
    {
        IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
        incidentParms.forced = true;
        incidentParms.raidStrategy = Snowstorm_MiscDefOf.OAGene_SnowstormImmediateAttackBreaching;
        incidentParms.faction = faction;

        return incidentParms;
    }
}
