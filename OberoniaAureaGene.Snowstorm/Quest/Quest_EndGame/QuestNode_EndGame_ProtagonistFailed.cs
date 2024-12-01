using RimWorld.QuestGen;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_Failed : QuestNode
{
    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }

    protected override void RunInt()
    {
        Snowstorm_StoryUtility.StoryGameComp?.Notify_StroyFailed();
    }
}
