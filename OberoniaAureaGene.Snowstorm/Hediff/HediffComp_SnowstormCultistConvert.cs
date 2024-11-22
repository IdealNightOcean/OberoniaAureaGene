using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_SnowstormCultistConvert : HediffCompProperties
{
    public IntRange convertInterval;
    public HediffCompProperties_SnowstormCultistConvert()
    {
        compClass = typeof(HediffComp_SnowstormCultistConvert);
    }
}

public class HediffComp_SnowstormCultistConvert : HediffComp
{
    public HediffCompProperties_SnowstormCultistConvert Props => props as HediffCompProperties_SnowstormCultistConvert;

    protected int ticksRemaining = 600;
    protected const int ConvertInterval = 1200;

    [Unsaved]
    GameComponent_Snowstorm snowstormGameComp;
    GameComponent_Snowstorm SnowstormGameComp => snowstormGameComp ??= Snowstorm_MiscUtility.SnowstormGameComp;
    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            GameComponent_Snowstorm snowstormGameComp = SnowstormGameComp;
            if (snowstormGameComp == null || !snowstormGameComp.CanCultistConvertNow)
            {
                ticksRemaining = Props.convertInterval.RandomInRange;
                return;
            }
            if (TryConvert(parent.pawn))
            {
                snowstormGameComp.nextCultistConvertTick = Find.TickManager.TicksGame + ConvertInterval;
            }
            ticksRemaining = Props.convertInterval.RandomInRange;
        }
    }

    protected static bool TryConvert(Pawn preacher)
    {
        IEnumerable<Pawn> pawns = preacher.Map?.mapPawns.FreeColonistsSpawned.Where(p => p.Awake());
        if (pawns == null || !pawns.Any())
        {
            return false;
        }
        foreach (Pawn pawn in pawns)
        {
            pawn.needs.mood?.thoughts.memories.TryGainMemory(Snowstrom_ThoughtDefOf.OAGene_Thought_SnowstormCultistConvert);
            if (ModsConfig.IdeologyActive)
            {
                float certaintyLoss = pawn.GetStatValue(StatDefOf.CertaintyLossFactor) * 0.025f * -1f;
                pawn.ideo?.Reassure(certaintyLoss);
            }
        }
        return true;
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
    }
}