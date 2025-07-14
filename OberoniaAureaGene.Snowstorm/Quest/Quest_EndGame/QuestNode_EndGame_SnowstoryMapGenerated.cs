using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_SnowstoryMapGenerated : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;
    protected override bool TestRunInt(Slate slate)
    {
        GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
        if (storyGameComp is null || storyGameComp.storyInProgress || storyGameComp.storyFinished)
        {
            return false;
        }
        return true;
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        if (slate.Get<WorldObject>("hometown") is null)
        {
            return;
        }
        QuestPart_EndGame_SnowstoryMapGenerated questPart_EndGame_SnowstoryMapGenerated = new()
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal")
        };
        QuestGen.quest.AddPart(questPart_EndGame_SnowstoryMapGenerated);
    }
}