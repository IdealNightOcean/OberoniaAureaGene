using RimWorld.QuestGen;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_SnowstoryMapGenerated : QuestNode
{
    protected override bool TestRunInt(Slate slate)
    {
        GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
        if (storyGameComp == null || storyGameComp.storyInProgress)
        {
            return false;
        }
        return true;
    }

    protected override void RunInt()
    {
        Snowstorm_StoryUtility.StoryGameComp?.Notify_StoryInProgress();
    }
}