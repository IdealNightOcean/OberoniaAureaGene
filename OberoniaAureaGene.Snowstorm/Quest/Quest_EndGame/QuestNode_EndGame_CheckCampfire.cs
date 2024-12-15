using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_CheckCampfire : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;
    [NoTranslate]
    public SlateRef<string> inSignalDisable;
    [NoTranslate]
    public SlateRef<string> outSignalNoValidCampfire;

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
        QuestPart_EndGame_CheckCampfire questPart_EndGame_CheckCampfire = new()
        {
            inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            inSignalDisable = QuestGenUtility.HardcodedSignalWithQuestID(inSignalDisable.GetValue(slate)),
            outSignalNoValidCampfire = QuestGenUtility.HardcodedSignalWithQuestID(outSignalNoValidCampfire.GetValue(slate)),
            hometown = hometown
        };
        questPart_EndGame_CheckCampfire.SetFirstCheckDelay(60000);
        QuestGen.quest.AddPart(questPart_EndGame_CheckCampfire);
    }
}
