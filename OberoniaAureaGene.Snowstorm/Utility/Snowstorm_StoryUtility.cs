
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

    public static bool OnlyProtagonist = false;
    public static Pawn OtherPawn = null;

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
        Map hometownMap = StoryGameComp.hometownMap;
        if (hometownMap == null)
        {
            MapParent hometown = StoryGameComp.hometown;
            hometown ??= Find.WorldObjects.AllWorldObjects.Where(o => o.def == Snowstorm_MiscDefOf.OAGene_Hometown).FirstOrFallback() as MapParent;
            if (hometown != null && hometown.HasMap)
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
        GameComponent_SnowstormStory storyGameComp = StoryGameComp;
        if (storyGameComp == null || !storyGameComp.StoryActive)
        {
            TryLogFailMessage("Try fire snowstorm end-game quest but StoryGameComp is NULL or inactive.");
            return false;
        }
        if (storyGameComp.hometownSpawned || storyGameComp.storyInProgress)
        {
            TryLogFailMessage("Try fire snowstorm end-game quest but end-game quest is already ongoing.");
            return false;
        }
        if (storyGameComp.storyFinished)
        {
            TryLogFailMessage("Try fire snowstorm end-game quest but end-game quest has been accomplished.");
            return false;
        }
        if (storyGameComp.Protagonist == null || storyGameComp.Protagonist.Dead)
        {
            TryLogFailMessage("Try fire snowstorm end-game quest but StoryGameComp is NULL or inactive.");
            return false;
        }

        Log.Message("The end-game quest triggering passed the StoryGameComp validity test.".Colorize(Color.green));
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
        string victoryText = string.Empty;
        if (OnlyProtagonist || OtherPawn == null)
        {
            victoryText = "OAGene_ReturnHome_Single".Translate(protagonist.Named("PAWN"));
        }
        else
        {
            victoryText = "OAGene_ReturnHome_Muti".Translate(protagonist.Named("PAWN"), OtherPawn.Named("OTHER"));
        }


        GameVictoryUtility.ShowCredits(victoryText, null, exitToMainMenu: false, SongStartDelay);

    }
}