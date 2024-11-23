
using Verse;

namespace OberoniaAureaGene.Snowstorm;


[StaticConstructorOnStartup]
public static class Snowstorm_StoryUtility
{
    public static GameComponent_SnowstormStory StoryGameComp => Current.Game.GetComponent<GameComponent_SnowstormStory>();

    public static bool StoryActive => StoryGameComp?.StoryActive ?? false;
    public static bool TryGetActivedStoryComp(out GameComponent_SnowstormStory storyComp)
    {
        storyComp = StoryGameComp;
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

    public static bool IsStoryProtagonist(Pawn pawn)
    {
        if (TryGetActivedStoryComp(out GameComponent_SnowstormStory storyComp))
        {
            return pawn == storyComp.Protagonist;
        }
        return false;
    }
}