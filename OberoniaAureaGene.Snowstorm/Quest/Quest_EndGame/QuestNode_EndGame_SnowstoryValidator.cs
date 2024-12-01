using RimWorld;
using RimWorld.QuestGen;


namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_SnowstoryValidator : QuestNode
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
            if (storyGameComp != null && storyGameComp.Protagonist != null)
            {
                Slate slate = QuestGen.slate;
                slate.Set("protagonist", storyGameComp.Protagonist);
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
        if (storyGameComp.hometownSpawned || storyGameComp.storyInProgress || storyGameComp.storyFinished)
        {
            return false;
        }
        if (storyGameComp.Protagonist == null || storyGameComp.Protagonist.Dead)
        {
            return false;
        }
        return true;
    }
}
