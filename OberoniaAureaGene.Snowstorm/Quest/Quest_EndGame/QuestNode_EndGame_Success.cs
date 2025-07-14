using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_Success : QuestNode
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
        QuestPart_EndGame_Success questPart_EndGame_Success = new()
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            protagonist = protagonist,
            hometown = hometown
        };
        QuestGen.quest.AddPart(questPart_EndGame_Success);
    }
}
