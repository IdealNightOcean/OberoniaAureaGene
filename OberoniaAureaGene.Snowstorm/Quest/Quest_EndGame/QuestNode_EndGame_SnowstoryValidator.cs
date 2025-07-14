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
        GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
        if (storyGameComp is not null && storyGameComp.Protagonist is not null)
        {
            Slate slate = QuestGen.slate;
            slate.Set("protagonist", storyGameComp.Protagonist);
        }
    }
}
