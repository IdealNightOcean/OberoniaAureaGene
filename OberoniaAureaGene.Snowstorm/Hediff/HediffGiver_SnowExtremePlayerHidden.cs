using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowExtremePlayerHidden : HediffGiver
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
        if (ModsConfig.AnomalyActive && p.IsMutant)
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