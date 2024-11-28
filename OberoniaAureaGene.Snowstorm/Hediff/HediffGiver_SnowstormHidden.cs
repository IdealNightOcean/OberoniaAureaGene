using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowstormHidden : HediffGiver
{
    public List<HediffDef> hediffs;
    public float mtbDays;

    [Unsaved]
    protected GameComponent_Snowstorm snowstormGameComp;

    protected GameComponent_Snowstorm SnowstormGameComp => snowstormGameComp ??= Snowstorm_MiscUtility.SnowstormGameComp;

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
            if (!SnowstormGameComp.CanGetSnowstormMentalNow)
            {
                return;
            }
            if (ChanceFactor(pawn) > 0f)
            {
                HediffDef giverHediff = hediffs.RandomElement();
                if (TryApplyHediff(pawn, giverHediff))
                {
                    int ticksGame = Find.TickManager.TicksGame;
                    causeSnow.nextGetHediffTick = ticksGame + 180000;
                    SnowstormGameComp.nextSnowstormMentalTick = ticksGame + 60000;
                }
            }
        }
    }
    public bool TryApplyHediff(Pawn pawn, HediffDef giveHediff, List<Hediff> outAddedHediffs = null)
    {
        if (pawn.ageTracker.CurLifeStage == LifeStageDefOf.HumanlikeBaby)
        {
            if (Find.Storyteller.difficulty.babiesAreHealthy || giveHediff == Snowstrom_HediffDefOf.OAGene_Hediff_SnowstormAngry)
            {
                return false;
            }
        }
        if (pawn.genes != null && !pawn.genes.HediffGiversCanGive(giveHediff))
        {
            return false;
        }
        return HediffGiverUtility.TryApply(pawn, giveHediff, partsToAffect, canAffectAnyLivePart, countToAffect, outAddedHediffs);
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
