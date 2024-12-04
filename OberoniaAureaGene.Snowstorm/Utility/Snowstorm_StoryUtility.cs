
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;


[StaticConstructorOnStartup]
public static class Snowstorm_StoryUtility
{
    public static GameComponent_SnowstormStory StoryGameComp => Current.Game.GetComponent<GameComponent_SnowstormStory>();

    public static bool StoryActive => StoryGameComp.StoryActive;

    public static bool TryGetStoryProtagonist(out Pawn protagonist)
    {
        protagonist = StoryGameComp.Protagonist;
        return protagonist != null;
    }

    public static bool IsStoryProtagonist(Pawn pawn)
    {
        if (pawn == null)
        {
            return false;
        }
        return pawn == StoryGameComp.Protagonist;
    }


    public static void EndGame(Pawn protagonist, bool onlyProtagonist)
    {
        GameVictoryUtility.ShowCredits("OAGene_ReturnHome".Translate(protagonist.Named("PAWN")), null);

    }
}