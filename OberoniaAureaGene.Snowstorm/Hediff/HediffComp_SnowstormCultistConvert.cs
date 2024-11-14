using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_SnowstormCultistConvert : HediffCompProperties
{
    public HediffCompProperties_SnowstormCultistConvert()
    {
        compClass = typeof(HediffComp_SnowstormCultistConvert);
    }
}

public class HediffComp_SnowstormCultistConvert : HediffComp
{
    protected static int NextConvertTick = -1;
    protected static readonly int ConvertInterval = 5000;
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn.IsHashIntervalTick(250))
        {
            if (Rand.Chance(0.2f) && Find.TickManager.TicksGame > NextConvertTick)
            {
                if (TryConvert(parent.pawn))
                {
                    NextConvertTick = Find.TickManager.TicksGame + ConvertInterval;
                }
            }
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
        Scribe_Values.Look(ref NextConvertTick, "NextConvertTick", -1);
    }
}