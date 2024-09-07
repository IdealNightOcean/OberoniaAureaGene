using Verse;

namespace OberoniaAureaGene;
public class HediffGiver_ColdImmersion : HediffGiver
{
    public static readonly SimpleCurve ImmersionAdjustmentCurve =
    [
        new CurvePoint(-9999f, 0f),
        new CurvePoint(0f, 0f),
        new CurvePoint(1f, -0.6f),
        new CurvePoint(20f, -2.4f),
        new CurvePoint(9999f, -2.4f)
    ];
    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        if (ActiveHediff(pawn))
        {
            HealthUtility.AdjustSeverity(pawn, hediff, 0.0012f);
            return;
        }

        float ambientTemperature = pawn.AmbientTemperature;
        if (ambientTemperature > 0)
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
            if (firstHediffOfDef != null)
            {
                float sevOffest = ImmersionAdjustmentCurve.Evaluate(ambientTemperature);
                sevOffest *= 0.001f;
                firstHediffOfDef.Severity += sevOffest;
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
        if (map.weatherManager.curWeather != OberoniaAureaGeneDefOf.OAGene_SnowExtreme)
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