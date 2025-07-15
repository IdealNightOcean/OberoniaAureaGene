using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class RaidStrategyWorker_SnowstormAttackBreaching : RaidStrategyWorker_ImmediateAttackBreaching
{
    public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
    {
        if (GameComponent_SnowstormStory.Instance is null || !GameComponent_SnowstormStory.Instance.StoryActive)
        {
            return false;
        }

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
