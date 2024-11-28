using RimWorld;
using RimWorld.QuestGen;


namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_SnowstoryEndGame_Validator : QuestNode
{
    protected override bool TestRunInt(Slate slate)
    {
        return SnowstoryEndGame_Validator();
    }
    protected override void RunInt()
    {
        if (SnowstoryEndGame_Validator())
        {
            GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
            if (storyGameComp != null)
            {
                storyGameComp.storyStart = true;
            }
        }
    }

    private static bool SnowstoryEndGame_Validator()
    {
        if (GenDate.DaysPassed < 10)
        {
            return false;
        }
        GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
        if (storyGameComp == null || !storyGameComp.StoryActive)
        {
            return false;
        }
        if (storyGameComp.storyStart || storyGameComp.storyFinished)
        {
            return false;
        }
        return true;
    }
}
