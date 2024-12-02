using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_SnowstoryMapGenerated : QuestNode
{
    [NoTranslate]
    public SlateRef<string> storeMapAs = "hometownMap";

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
        Slate slate = QuestGen.slate;
        MapParent hometown = slate.Get<WorldObject>("hometown") as MapParent;
        if (hometown == null || hometown.Map == null)
        {
            return;
        }
        slate.Set(storeMapAs.GetValue(slate), hometown.Map);
        Snowstorm_StoryUtility.StoryGameComp?.Notify_StoryInProgress();
    }
}