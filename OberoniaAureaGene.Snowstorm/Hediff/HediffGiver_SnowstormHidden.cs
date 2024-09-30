using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowstormHidden : HediffGiver
{
    public override float ChanceFactor(Pawn pawn)
    {
        TraitSet traitSet = pawn.story?.traits;
        if (traitSet != null)
        {
            for (int i = 0; i < traitSet.allTraits.Count; i++)
            {
                Trait trait = traitSet.allTraits[i];
                if (IsSpecialTrait(trait))
                {
                    return 0f;
                }
            }
        }
        return 1f;
    }

    private static bool IsSpecialTrait(Trait t)
    {
        if (t.def == OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor)
        {
            return true;
        }
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
