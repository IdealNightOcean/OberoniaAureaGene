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
        if (ActiveGiver(pawn))
        {
            float sevOffest = 1.2f;
            HealthUtility.AdjustSeverity(pawn, hediff, sevOffest);

            return;
        }

        float ambientTemperature = pawn.AmbientTemperature;
        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
        if (ambientTemperature > 0 && firstHediffOfDef != null)
        {
            float sevOffest = ImmersionAdjustmentCurve.Evaluate(ambientTemperature);

            firstHediffOfDef.Severity += sevOffest;
        }
    }
    public static bool ActiveGiver(Pawn p)
    {
        if (!p.Spawned || p.Dead)
        {
            return false;
        }
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