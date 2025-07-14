using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_EndEndGameSnowstorm : QuestNode
{

    [NoTranslate]
    public SlateRef<string> inSignal;

    public SlateRef<int> snowstormEndDelay;
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
        QuestPart_EndGame_EndEndGameSnowstorm questPart_EndGame_EndEndGameSnowstorm = new()
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            snowstormEndDelay = snowstormEndDelay.GetValue(slate),
        };
        QuestGen.quest.AddPart(questPart_EndGame_EndEndGameSnowstorm);
    }

}

