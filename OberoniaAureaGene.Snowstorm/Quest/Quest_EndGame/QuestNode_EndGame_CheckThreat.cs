using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_CheckThreat : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;
    [NoTranslate]
    public SlateRef<string> outSignalNoThreat;

    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        MapParent hometown = slate.Get<WorldObject>("hometown") as MapParent;
        if (hometown is null)
        {
            return;
        }
        QuestPart_EndGame_CheckThreat questPart_EndGame_CheckThreat = new()
        {
            inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            outSignalNoThreat = QuestGenUtility.HardcodedSignalWithQuestID(outSignalNoThreat.GetValue(slate)),
            hometown = hometown
        };
        QuestGen.quest.AddPart(questPart_EndGame_CheckThreat);
    }
}
