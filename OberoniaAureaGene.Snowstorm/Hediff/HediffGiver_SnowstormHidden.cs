using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowstormHidden : HediffGiver
{
    public float mtbDays;

    [Unsaved]
    protected GameComponent_Snowstorm snowstorm_GC;

    protected GameComponent_Snowstorm Snowstorm_GC => snowstorm_GC ??= Current.Game.GetComponent<GameComponent_Snowstorm>();

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

    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        if (Snowstorm_GC.lastSnowstormMentalTick < Find.TickManager.TicksGame + 30000)
        {
            return;
        }
        float mtbDays = this.mtbDays;
        float chance = ChanceFactor(pawn);
        if (chance > 0f && Rand.Value < 1f / (mtbDays * 60000f))
        {
            if (TryApply(pawn))
            {
                Snowstorm_GC.lastSnowstormMentalTick = Find.TickManager.TicksGame;
            }
        }
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
