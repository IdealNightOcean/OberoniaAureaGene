using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class HediffGiver_SnowExtreme : HediffGiver
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
            pawn.health.GetOrAddHediff(snowExtremeHediff);
            if (pawn.health.hediffSet.BleedRateTotal > 0.001f)
            {
                pawn.health.GetOrAddHediff(frozenWoundHediff);
            }
            if (ambientTemperature < OAGeneUtility.ComfyTemperatureMin(pawn))
            {
                pawn.health.GetOrAddHediff(coldSnowHediff);
            }
            else
            {
                RemoveFirstHediffOfDef(pawn, coldSnowHediff);
            }
            HealthUtility.AdjustSeverity(pawn, coldImmersionHediff, 0.0012f);
        }
        else
        {
            RemoveFirstHediffOfDef(pawn, snowExtremeHediff);
            RemoveFirstHediffOfDef(pawn, coldSnowHediff);
            if (ambientTemperature > 0)
            {
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(coldImmersionHediff);
                if (firstHediffOfDef != null)
                {
                    float sevAdjuest = ImmersionAdjustmentCurve.Evaluate(ambientTemperature) * 0.001f;
                    firstHediffOfDef.Severity += sevAdjuest;
                }
            }
        }
    }

    public static bool ActiveHediff(Pawn p)
    {
        Map map = p.Map;
        if (map == null)
        {
            return false;
        }
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
    private static void RemoveFirstHediffOfDef(Pawn p, HediffDef hediffDef)
    {
        Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(hediffDef);
        if (firstHediffOfDef != null)
        {
            p.health.RemoveHediff(firstHediffOfDef);
        }
    }
}
