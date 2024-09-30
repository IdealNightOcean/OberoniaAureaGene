using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowExtremePlayerHiden : HediffGiver
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
        if (p.IsMutant)
        {
            return false;
        }
        if (p.Faction == null || !p.Faction.IsPlayer)
        {
            if (!p.IsPrisoner)
            {
                return false;
            }
        }
        if (!OAGeneUtility.IsSnowExtremeWeather(p.Map))
        {
            return false;
        }
        return true;
    }

}