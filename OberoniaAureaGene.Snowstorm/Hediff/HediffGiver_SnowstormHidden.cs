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

    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        if (Rand.Value < 1f / (mtbDays * 2500f))
        {
            if (cause is not Hediff_SnowExtremePlayerHidden causeSnow || !causeSnow.CanGetHediffNow)
            {
                return;
            }
            if (!SnowstormGameComp.CanGetSnowstormMentalNow)
            {
                return;
            }
            if (CanApplyHediff(pawn))
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
    protected static bool CanApplyHediff(Pawn pawn)
    {
        if (pawn.ageTracker.CurLifeStage == LifeStageDefOf.HumanlikeBaby)
        {
            return false;
        }
        if (pawn.health.hediffSet.GetFirstHediffOfDef(Snowstorm_HediffDefOf.OAGene_Hediff_SnowstormOblivious) != null)
        {
            return false;
        }
        TraitSet traitSet = pawn.story?.traits;
        if (traitSet != null)
        {
            for (int i = 0; i < traitSet.allTraits.Count; i++)
            {
                Trait trait = traitSet.allTraits[i];
                if (IsSpecialTrait(trait))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool TryApplyHediff(Pawn pawn, HediffDef giveHediff)
    {
        if (pawn.genes != null && !pawn.genes.HediffGiversCanGive(giveHediff))
        {
            return false;
        }
        return HediffGiverUtility.TryApply(pawn, giveHediff, partsToAffect, canAffectAnyLivePart, countToAffect, null);
    }

    private static bool IsSpecialTrait(Trait t)
    {
        if (t.def == OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor)
        {
            return true;
        }
        if (t.def == OAGene_MiscDefOf.Faith)
        {
            return !t.Suppressed;
        }
        if (t.def == OAGene_RimWorldDefOf.Nerves && t.Degree >= 1)
        {
            return !t.Suppressed;
        }
        return false;
    }
}
