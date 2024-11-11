using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_SnowExtreme : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.Spawned || !OAGeneUtility.IsSnowExtremeWeather(p.Map))
        {
            return ThoughtState.Inactive;
        }

        TraitSet traitSet = p.story.traits;
        if (traitSet != null)
        {
            for (int i = 0; i < traitSet.allTraits.Count; i++)
            {
                Trait trait = traitSet.allTraits[i];
                if (IsSpecialTrait(trait))
                {
                    return ThoughtState.ActiveAtStage(1);
                }
            }
        }
        return ThoughtState.ActiveAtStage(0);

    }
    private static bool IsSpecialTrait(Trait t)
    {
        if (t.def == OAGene_RimWorldDefOf.Faith)
        {
            return !t.Suppressed;
        }
        if (t.def == OAGene_RimWorldDefOf.Nerves && (t.Degree == 0 || t.Degree == 1))
        {
            return !t.Suppressed;
        }
        return false;
    }

}
