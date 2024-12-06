using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_MakeGameCondition_SnowstormFog : IncidentWorker_MakeGameCondition
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (parms.target is not Map map)
        {
            return false;
        }
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        return base.CanFireNowSub(parms);
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (parms.target is not Map map)
        {
            return false;
        }
        GameConditionManager gameConditionManager = parms.target.GameConditionManager;
        GameCondition_ExtremeSnowstorm snowstorm = gameConditionManager.GetActiveCondition<GameCondition_ExtremeSnowstorm>();
        if (snowstorm == null)
        {
            return false;
        }
        GameConditionDef gameConditionDef = GetGameConditionDef(parms);
        int duration = snowstorm.TicksLeft;
        GameCondition gameCondition = GameConditionMaker.MakeCondition(gameConditionDef, duration);
        gameConditionManager.RegisterCondition(gameCondition);
        if (!def.letterLabel.NullOrEmpty() && !gameCondition.def.letterText.NullOrEmpty() && !gameCondition.HiddenByOtherCondition(map))
        {
            parms.letterHyperlinkThingDefs = gameCondition.def.letterHyperlinks;
            SendStandardLetter(def.letterLabel, gameCondition.LetterText, def.letterDef, parms, LookTargets.Invalid);
        }

        return true;
    }

}
