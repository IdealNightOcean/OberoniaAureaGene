using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class HediffGiver_SnowExtreme : HediffGiver
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
        bool active = ActiveHediff(pawn);
        float ambientTemperature = pawn.AmbientTemperature;
        if (active)
        {
            pawn.health.GetOrAddHediff(OAGene_HediffDefOf.OAGene_Hediff_SnowExtreme);
            if (ambientTemperature < pawn.SafeTemperatureRange().min)
            {
                pawn.health.GetOrAddHediff(OAGene_HediffDefOf.OAGene_Hediff_ColdSnow);
            }
            else
            {
                RemoveFirstHediffOfDef(pawn, OAGene_HediffDefOf.OAGene_Hediff_ColdSnow);
            }
            HealthUtility.AdjustSeverity(pawn, OAGene_HediffDefOf.OAGene_Hediff_ColdImmersion, 0.0012f);
        }
        else
        {
            RemoveFirstHediffOfDef(pawn, OAGene_HediffDefOf.OAGene_Hediff_SnowExtreme);
            RemoveFirstHediffOfDef(pawn, OAGene_HediffDefOf.OAGene_Hediff_ColdSnow);
            if (ambientTemperature > 0)
            {
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(OAGene_HediffDefOf.OAGene_Hediff_ColdImmersion);
                if (firstHediffOfDef != null)
                {

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
    private static void RemoveFirstHediffOfDef(Pawn p, HediffDef hediffDef)
    {
        Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(hediffDef);
        if (firstHediffOfDef != null)
        {
            p.health.RemoveHediff(firstHediffOfDef);
        }
    }
}
