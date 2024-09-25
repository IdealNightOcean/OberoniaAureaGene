using Verse;

namespace OberoniaAureaGene;

public class HediffGiver_SnowExtremeHumanlickHide : HediffGiver
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
        if (!p.RaceProps.Humanlike || p.IsMutant)
        {
            return false;
        }
        Map map = p.Map;
        if (map == null)
        {
            return false;
        }
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        return true;
    }

}