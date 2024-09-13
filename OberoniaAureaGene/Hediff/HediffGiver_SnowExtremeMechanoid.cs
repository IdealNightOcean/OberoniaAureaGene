using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class HediffGiver_SnowExtremeMechanoid : HediffGiver
{
    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
        bool active = ActiveHediff(pawn);
        if (active)
        {
            if (firstHediffOfDef == null)
            {
                pawn.health.AddHediff(hediff);
            }
        }
        else if (firstHediffOfDef != null)
        {
            pawn.health.RemoveHediff(firstHediffOfDef);
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
}
