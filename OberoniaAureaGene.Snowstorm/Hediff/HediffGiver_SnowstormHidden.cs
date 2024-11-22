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
        if (Rand.Value < 1f / (mtbDays * 1000f))
        {
            if (cause is not Hediff_SnowExtremePlayerHidden causeSnow || !causeSnow.CanGetHediffNow)
            {
                return;
            }
            if (!Snowstorm_GC.CanGetSnowstormMentalNow)
            {
                return;
            }
            if (ChanceFactor(pawn) > 0f)
            {
                if (TryApply(pawn))
                {
                    int ticksGame = Find.TickManager.TicksGame;
                    causeSnow.nextGetHediffTick = ticksGame + 180000;
                    Snowstorm_GC.nextSnowstormMentalTick = ticksGame + 60000;
                }
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
