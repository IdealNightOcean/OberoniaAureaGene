
using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
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

    public static bool CanFireSnowstormEndGameNow(bool logMessage)
    {
        if (StoryGameComp is null || !StoryGameComp.StoryActive)
        {
            TryLogMessage($"[OAGene] 尝试触发长路归乡任务，但 {nameof(StoryGameComp)} 为 NULL 或未激活。");
            return false;
        }
        if (StoryGameComp.hometownSpawned || StoryGameComp.storyInProgress)
        {
            TryLogMessage("[OAGene] 尝试触发长路归乡任务，但长路归乡任务已在进行中。");
            return false;
        }
        if (StoryGameComp.storyFinished)
        {
            TryLogMessage("[OAGene] 尝试触发长路归乡任务，但长路归乡任务已完成。");
            return false;
        }
        if (StoryGameComp.Protagonist is null || StoryGameComp.Protagonist.Dead)
        {
            TryLogMessage("[OAGene] 尝试触发长路归乡任务，但 遗孤主角 为 NULL 或已死亡。");
            return false;
        }
        if (GenDate.DaysPassed < 10)
        {
            TryLogMessage("[OAGene] 尝试触发长路归乡任务，距游戏开始不足10日。");
            return false;
        }

        if (logMessage)
            Log.Message($"[OAGene] 结局任务触发已通过 {nameof(StoryGameComp)} 有效性测试。".Colorize(Color.green));

        return true;

        void TryLogMessage(string message)
        {
            if (logMessage) Log.Message(message.Colorize(Color.cyan));
        }
    }

    public static bool TryTriggerSnowstormEndGame(bool logMessage)
    {
        if (!CanFireSnowstormEndGameNow(logMessage: logMessage))
        {
            if (logMessage)
            {
                Log.Message($"[OARO] 长路归乡任务触发失败：未能通过 {nameof(StoryGameComp)} 有效性测试".Colorize(ColorLibrary.RedReadable));
            }
            return false;
        }

        bool result = OAFrame_QuestUtility.TryGenerateQuestAndMakeAvailable(
               quest: out Quest quest,
               scriptDef: Snowstorm_MiscDefOf.OAGene_EndGame_Homecoming,
               slate: new Slate());

        if (logMessage)
        {
            if (result)
            {
                Log.Message("[OARO] 长路归乡任务触发成功".Colorize(Color.green));
            }
            else
            {
                Log.Message("[OARO] 长路归乡任务触发失败".Colorize(ColorLibrary.RedReadable));
            }
        }

        return result;
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