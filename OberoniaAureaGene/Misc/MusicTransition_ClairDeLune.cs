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
            if (map.gameConditionManager.ConditionIsActive(OAGene_MiscDefOf.OAGene_ExtremeSnowstorm))
            {
                return true;
            }
        }
        return false;
    }
}
