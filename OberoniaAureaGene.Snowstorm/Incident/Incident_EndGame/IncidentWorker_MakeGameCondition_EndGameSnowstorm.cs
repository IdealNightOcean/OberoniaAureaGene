using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class IncidentWorker_MakeGameCondition_EndGameSnowstorm : IncidentWorker_MakeGameCondition
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        Map hometownMap = parms.target as Map;
        hometownMap ??= Snowstorm_StoryUtility.GetHometownMap();
        if (hometownMap == null)
        {
            return false;
        }
        GameConditionManager gameConditionManager = Find.World.GameConditionManager;
        if (gameConditionManager == null)
        {
            Log.ErrorOnce($"Couldn't find condition manager for incident target {Find.World}", 70849667);
            return false;
        }

        GameConditionDef gameConditionDef = GetGameConditionDef(parms);
        if (gameConditionDef == null)
        {
            return false;
        }

        if (gameConditionManager.ConditionIsActive(gameConditionDef))
        {
            return false;
        }

        return true;
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        Map hometownMap = parms.target as Map;
        hometownMap ??= Snowstorm_StoryUtility.GetHometownMap();
        if (hometownMap == null)
        {
            return false;
        }
        GameConditionManager gameConditionManager = Find.World.GameConditionManager;
        GameConditionDef gameConditionDef = GetGameConditionDef(parms);
        int duration = Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f);
        GameCondition_EndGame_ExtremeSnowstorm gameCondition = GameConditionMaker.MakeCondition(gameConditionDef, duration) as GameCondition_EndGame_ExtremeSnowstorm;
        if (gameCondition == null)
        {
            return false;
        }
        gameCondition.SetMainMap(hometownMap);
        gameConditionManager.RegisterCondition(gameCondition);
        if (!def.letterLabel.NullOrEmpty() && !gameCondition.def.letterText.NullOrEmpty())
        {
            parms.letterHyperlinkThingDefs = gameCondition.def.letterHyperlinks;
            SendStandardLetter(def.letterLabel, gameCondition.LetterText, def.letterDef, parms, LookTargets.Invalid);
        }

        return true;
    }
}
