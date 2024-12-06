
using RimWorld;
using RimWorld.Planet;
using System.Linq;
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

    public static Map GetHometownMap()
    {
        MapParent hometown = Find.WorldObjects.AllWorldObjects.Where(o => o.def == Snowstrom_MiscDefOf.OAGene_Hometown).FirstOrFallback() as MapParent;
        if (hometown != null && hometown.HasMap)
        {
            return hometown.Map;
        }
        return null;
    }

    public static void EndGame(Pawn protagonist, bool onlyProtagonist)
    {
        GameVictoryUtility.ShowCredits("OAGene_ReturnHome".Translate(protagonist.Named("PAWN")), null);

    }
}