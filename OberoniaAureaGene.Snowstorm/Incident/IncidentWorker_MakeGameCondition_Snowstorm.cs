using RimWorld;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_MakeGameCondition_Snowstorm : IncidentWorker_MakeGameCondition
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        return base.CanFireNowSub(parms);
    }
}
