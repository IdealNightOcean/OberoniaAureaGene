using RimWorld.QuestGen;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_IsSnowstoryScenario : QuestNode
{
    protected override bool TestRunInt(Slate slate)
    {
        return Snowstorm_StoryUtility.StoryActive;
    }
    protected override void RunInt() { }
}
