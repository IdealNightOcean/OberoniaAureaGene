using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class MusicTransition_ClairDeLune : MusicTransition
{
    public override bool IsTransitionSatisfied()
    {
        if (!base.IsTransitionSatisfied())
        {
            return false;
        }
        foreach (Map map in Find.Maps)
        {
            if(!map.IsPlayerHome)
            {
                continue;
            }

            GameCondition_ExtremeSnowstorm snowstorm = (GameCondition_ExtremeSnowstorm)map.gameConditionManager.GetActiveCondition(OAGene_MiscDefOf.OAGene_ExtremeSnowstorm);
            if (snowstorm != null)
            {
                return snowstorm.causeColdSnap;
            }
        }
        return false;
    }
}
