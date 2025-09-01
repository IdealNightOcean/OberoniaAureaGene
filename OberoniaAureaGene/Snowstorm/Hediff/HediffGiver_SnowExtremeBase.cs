using OberoniaAurea_Frame;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class HediffGiver_SnowExtremeBase : HediffGiver
{
    public HediffDef snowExtremeHediff;
    public HediffDef coldSnowHediff;
    public HediffDef coldImmersionHediff;
    public HediffDef frozenWoundHediff;

    protected static readonly SimpleCurve ImmersionAdjustmentCurve =
    [
        new CurvePoint(-9999f, 0f),
        new CurvePoint(0f, 0f),
        new CurvePoint(1f, -0.6f),
        new CurvePoint(20f, -2.4f),
        new CurvePoint(9999f, -2.4f)
    ];
    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        bool active = ActiveHediff(pawn);
        float ambientTemperature = pawn.AmbientTemperature;
        if (active)
        {
            TryActiveHediff(pawn, ambientTemperature);
        }
        else
        {
            TryInactiveHediff(pawn, ambientTemperature);
        }
    }

    protected virtual void TryActiveHediff(Pawn pawn, float ambientTemperature)
    {
        Pawn_HealthTracker pawnHealth = pawn.health;
        pawn.AdjustOrAddHediff(snowExtremeHediff, -1, 250);
        //伤口冻结
        if (pawnHealth.hediffSet.BleedRateTotal > 0.001f)
        {
            pawnHealth.AddHediff(frozenWoundHediff);
        }
        //冷雪
        if (ambientTemperature < OAGeneUtility.ComfyTemperatureMin(pawn))
        {
            pawnHealth.AddHediff(coldSnowHediff);
        }
        //寒冷堆砌
        HealthUtility.AdjustSeverity(pawn, coldImmersionHediff, 0.0012f);
    }

    protected virtual void TryInactiveHediff(Pawn pawn, float ambientTemperature)
    {
        if (ambientTemperature > 0)
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(coldImmersionHediff);
            if (firstHediffOfDef is not null)
            {
                float sevAdjuest = ImmersionAdjustmentCurve.Evaluate(ambientTemperature) * 0.001f;
                firstHediffOfDef.Severity += sevAdjuest;
            }
        }
    }
    public static bool ActiveHediff(Pawn p)
    {
        Map map = p.Map;
        if (!OAGeneUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        if (map.roofGrid.Roofed(p.Position))
        {
            return false;
        }
        return true;
    }
}
