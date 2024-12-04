using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_Fail : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;

    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        QuestPart_EndGame_Fail questPart_EndGame_Fail = new()
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal")
        };
        QuestGen.quest.AddPart(questPart_EndGame_Fail);
    }
}
