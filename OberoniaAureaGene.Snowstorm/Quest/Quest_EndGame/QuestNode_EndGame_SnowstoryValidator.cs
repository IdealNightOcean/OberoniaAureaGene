using RimWorld.QuestGen;


namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_SnowstoryValidator : QuestNode
{
    protected override bool TestRunInt(Slate slate)
    {
        return Snowstorm_StoryUtility.CanFireSnowstormEndGameNow();
    }
    protected override void RunInt()
    {
        if (Snowstorm_StoryUtility.CanFireSnowstormEndGameNow())
        {
            GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
            if (storyGameComp != null && storyGameComp.Protagonist != null)
            {
                Slate slate = QuestGen.slate;
                slate.Set("protagonist", storyGameComp.Protagonist);
            }
        }
    }
}
