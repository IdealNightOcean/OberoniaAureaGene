
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public static class Snowstorm_StoryUtility
{
    private const float SongStartDelay = 2.5f;
    private static GameComponent_SnowstormStory StoryGameComp => GameComponent_SnowstormStory.Instance;

    public static bool OnlyProtagonist = true;
    public static Pawn OtherPawn = null;

    public static bool TryGetStoryProtagonist(out Pawn protagonist)
    {
        protagonist = StoryGameComp.Protagonist;
        return protagonist is not null;
    }

    public static bool IsStoryProtagonist(Pawn pawn)
    {
        if (pawn is null)
        {
            return false;
        }
        return pawn == StoryGameComp.Protagonist;
    }

    public static Map GetHometownMap()
    {
        Map hometownMap = StoryGameComp.hometownMap;
        if (hometownMap is null)
        {
            MapParent hometown = StoryGameComp.hometown;
            hometown ??= Find.WorldObjects.AllWorldObjects.Where(o => o.def == Snowstorm_MiscDefOf.OAGene_Hometown).FirstOrFallback() as MapParent;
            if (hometown is not null && hometown.HasMap)
            {
                hometownMap = hometown.Map;
            }
        }
        return hometownMap;
    }

    public static bool CanFireSnowstormEndGameNow(bool logFailMessage = true)
    {
        if (GenDate.DaysPassed < 10)
        {
            return false;
        }
        if (StoryGameComp is null || !StoryGameComp.StoryActive)
        {
            TryLogFailMessage("[OAGene] Try fire snowstorm end-game quest but StoryGameComp is NULL or inactive.");
            return false;
        }
        if (StoryGameComp.hometownSpawned || StoryGameComp.storyInProgress)
        {
            TryLogFailMessage("[OAGene] Try fire snowstorm end-game quest but end-game quest is already ongoing.");
            return false;
        }
        if (StoryGameComp.storyFinished)
        {
            TryLogFailMessage("[OAGene] Try fire snowstorm end-game quest but end-game quest has been accomplished.");
            return false;
        }
        if (StoryGameComp.Protagonist is null || StoryGameComp.Protagonist.Dead)
        {
            TryLogFailMessage("[OAGene] Try fire snowstorm end-game quest but StoryGameComp is NULL or inactive.");
            return false;
        }

        Log.Message("[OAGene] The end-game quest triggering passed the StoryGameComp validity test.".Colorize(Color.green));
        return true;

        void TryLogFailMessage(string failMessage)
        {
            if (logFailMessage)
            {
                Log.Message(failMessage.Colorize(Color.cyan));
            }
        }
    }

    public static void EndGame(Pawn protagonist)
    {
        string victoryText;
        if (OnlyProtagonist || OtherPawn is null)
        {
            victoryText = "OAGene_ReturnHome_Single".Translate(protagonist.Named("PAWN"));
        }
        else
        {
            victoryText = "OAGene_ReturnHome_Muti".Translate(protagonist.Named("PAWN"), OtherPawn.Named("OTHER"));
        }

        GameVictoryUtility.ShowCredits(victoryText, null, exitToMainMenu: false, SongStartDelay);
    }

    public static void ClearStaticCache()
    {
        OnlyProtagonist = true;
        OtherPawn = null;
    }
}