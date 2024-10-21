using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class Hediff_SnowstormInsomnia : HediffWithComps
{
    private static readonly HediffStage ActiveStage = new()
    {
        statFactors = [ new StatModifier()
        {
            stat = StatDefOf.RestRateMultiplier,
            value = 0.4f
        }]
    };

    private HediffStage curStage;
    public override HediffStage CurStage
    {
        get
        {
            if (pawn.IsHashIntervalTick(250))
            {
                float curMood = pawn.needs.mood?.CurLevel ?? 1f;
                if (curMood < 0.7f)
                {
                    curStage = ActiveStage;
                }
                else
                {
                    curStage = null;
                }
            }
            return curStage;
        }
    }
}