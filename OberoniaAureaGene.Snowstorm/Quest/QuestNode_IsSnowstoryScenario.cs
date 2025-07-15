using RimWorld.QuestGen;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_IsSnowstoryScenario : QuestNode
{
    protected override bool TestRunInt(Slate slate)
    {
        return GameComponent_SnowstormStory.Instance.StoryActive;
    }
    protected override void RunInt() { }
}
