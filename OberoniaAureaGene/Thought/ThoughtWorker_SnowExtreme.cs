using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_SnowExtreme : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.Spawned)
        {
            return ThoughtState.Inactive;
        }
        if (p.Map?.weatherManager.curWeather == OberoniaAureaGeneDefOf.OAGene_SnowExtreme)
        {
            TraitSet traitSet = p.story.traits;
            if (traitSet != null)
            {
                int stage;
                for (int i = 0; i < traitSet.allTraits.Count; i++)
                {
                    Trait trait = traitSet.allTraits[i];
                    if (IsSpecialTrait(trait, out stage))
                    {
                        return ThoughtState.ActiveAtStage(stage);
                    }
                }
            }
            return ThoughtState.ActiveAtStage(0);
        }
        return ThoughtState.Inactive;
    }
    private static bool IsSpecialTrait(Trait t, out int stage)
    {
        stage = 0;
        if (t.def == OberoniaAureaGeneDefOf.OAGene_ExtremeSnowSurvivor)
        {
            stage = 2;
            return true;
        }
        if (t.def == OAGene_RimWorldDefOf.Faith)
        {
            stage = 1;
            return !t.Suppressed;
        }
        if (t.def == OAGene_RimWorldDefOf.Nerves && (t.Degree == 0 || t.Degree == 1))
        {
            stage = 1;
            return !t.Suppressed;
        }
        return false;
    }

}
