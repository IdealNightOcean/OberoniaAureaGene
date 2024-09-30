using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class HediffGiver_SnowExtremeMechanoid : HediffGiver
{
    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        if (ActiveHediff(pawn))
        {
            pawn.health.AddHediff(hediff);
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
