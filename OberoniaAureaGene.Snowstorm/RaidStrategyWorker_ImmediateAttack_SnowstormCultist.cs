using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class RaidStrategyWorker_ImmediateAttack_SnowstormCultist : RaidStrategyWorker_ImmediateAttack_NeverFlee
{
    public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
    {
        GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
        if (storyGameComp == null || !storyGameComp.StoryActive || !storyGameComp.storyInProgress)
        {
            return false;
        };
        return base.CanUseWith(parms, groupKind);
    }

    public override float SelectionWeight(Map map, float basePoints)
    {
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return 0f;
        }
        return base.SelectionWeight(map, basePoints);
    }
}
