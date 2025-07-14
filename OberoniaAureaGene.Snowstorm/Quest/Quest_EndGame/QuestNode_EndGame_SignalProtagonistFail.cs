using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_SignalProtagonistFail : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;

    [NoTranslate]
    public SlateRef<string> protagonistLeftMapSignal;
    [NoTranslate]
    public SlateRef<string> protagonistDeadSignal;

    [NoTranslate]
    public SlateRef<string> outSignalFail;

    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        Pawn protagonist = slate.Get<Pawn>("protagonist");
        if (protagonist is null)
        {
            return;
        }
        MapParent hometown = slate.Get<WorldObject>("hometown") as MapParent;
        if (hometown is null)
        {
            return;
        }
        QuestPart_EndGame_SignalProtagonistFail questPart_EndGame_SignalProtagonistFail = new()
        {
            inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            protagonistLeftMapSignal = QuestGenUtility.HardcodedSignalWithQuestID(protagonistLeftMapSignal.GetValue(slate)),
            protagonistDeadSignal = QuestGenUtility.HardcodedSignalWithQuestID(protagonistDeadSignal.GetValue(slate)),
            outSignalFail = QuestGenUtility.HardcodedSignalWithQuestID(outSignalFail.GetValue(slate)),

            hometown = hometown,
            protagonist = protagonist
        };
        QuestGen.quest.AddPart(questPart_EndGame_SignalProtagonistFail);
    }

}
