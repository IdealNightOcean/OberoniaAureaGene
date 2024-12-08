using RimWorld;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_MakeGameCondition_EndGameCondition : IncidentWorker_MakeGameCondition
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        return base.CanFireNowSub(parms);
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        return base.TryExecuteWorker(parms);
    }
}