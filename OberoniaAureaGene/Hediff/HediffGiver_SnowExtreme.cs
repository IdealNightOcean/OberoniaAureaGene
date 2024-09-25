using OberoniaAurea_Frame;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class HediffGiver_SnowExtreme : HediffGiver
{
    public HediffDef snowExtremeHediff;
    public HediffDef coldSnowHediff;
    public HediffDef coldImmersionHediff;
    public HediffDef frozenWoundHediff;
    public HediffDef iceStormHediff;

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
            Pawn_HealthTracker pawnHealth = pawn.health;
            OberoniaAureaFrameUtility.AdjustOrAddHediff(pawn, snowExtremeHediff, overrideDisappearTicks: 250);
            //伤口冻结
            if (pawnHealth.hediffSet.BleedRateTotal > 0.001f)
            {
                OberoniaAureaFrameUtility.AdjustOrAddHediff(pawn, frozenWoundHediff, overrideDisappearTicks: 30000);
            }
            //冷雪
            if (ambientTemperature < OAGeneUtility.ComfyTemperatureMin(pawn))
            {
                OberoniaAureaFrameUtility.AdjustOrAddHediff(pawn, coldSnowHediff, overrideDisappearTicks: 250);
            }
            //冰晶暴风雪
            if (SnowstormUtility.IsIceStormWeather(pawn.Map))
            {
                OberoniaAureaFrameUtility.AdjustOrAddHediff(pawn, iceStormHediff, overrideDisappearTicks: 250);
            }
            //寒冷堆砌
            HealthUtility.AdjustSeverity(pawn, coldImmersionHediff, 0.0012f);
        }
        else
        {
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
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
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
