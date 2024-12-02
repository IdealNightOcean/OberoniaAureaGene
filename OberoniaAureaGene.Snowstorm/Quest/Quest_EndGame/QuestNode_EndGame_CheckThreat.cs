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
        Map hometownMap = slate.Get<Map>("hometownMap");
        if (hometownMap == null)
        {
            return;
        }
        QuestPart_EndGame_CheckThreat questPart_EndGame_CheckThreat = new()
        {
            inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            outSignalNoThreat = QuestGenUtility.HardcodedSignalWithQuestID(outSignalNoThreat.GetValue(slate)),
            map = hometownMap
        };
        QuestGen.quest.AddPart(questPart_EndGame_CheckThreat);
    }
}
