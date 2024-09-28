using Verse;

namespace OberoniaAureaGene;

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
        if (!SnowstormUtility.IsSnowExtremeWeather(p.Map))
        {
            return false;
        }
        return true;
    }

}