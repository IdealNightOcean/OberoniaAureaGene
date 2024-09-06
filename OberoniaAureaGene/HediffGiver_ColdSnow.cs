using Verse;

namespace OberoniaAureaGene;

public class HediffGiver_ColdSnow : HediffGiver
{
    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
        bool active = ActiveGiver(pawn);

        if (active && firstHediffOfDef == null)
        {
            pawn.health.AddHediff(hediff);
            return;
        }
        if (!active && firstHediffOfDef != null)
        {
            pawn.health.RemoveHediff(firstHediffOfDef);
            return;
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
        return p.AmbientTemperature < p.SafeTemperatureRange().min;
    }
}
