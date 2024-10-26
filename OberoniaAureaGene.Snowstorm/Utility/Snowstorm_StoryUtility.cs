
using Verse;

namespace OberoniaAureaGene.Snowstorm;


[StaticConstructorOnStartup]
public static class Snowstorm_StoryUtility
{
    public static bool StoryActive => Current.Game.GetComponent<GameComponent_SnowstormStory>()?.StoryActive ?? false;
    public static bool TryGetActivedStoryComp(out GameComponent_SnowstormStory storyComp)
    {
        storyComp = Current.Game.GetComponent<GameComponent_SnowstormStory>();
        return storyComp != null && storyComp.StoryActive;
    }
    public static bool TryGetStoryProtagonist(out Pawn protagonist)
    {
        protagonist = StoryProtagonist();
        return protagonist != null;
    }

    public static Pawn StoryProtagonist()
    {
        if (TryGetActivedStoryComp(out GameComponent_SnowstormStory storyComp))
        {
            return storyComp.Protagonist;
        }
        return null;
    }
}