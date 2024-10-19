using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class HediffGiver_IceRain : HediffGiver
{
    public HediffDef iceRainHediff;

    //冰晶暴风雪
    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        if (ActiveHediff(pawn))
        {
            pawn.health.AddHediff(iceRainHediff);
        }
    }

    public static bool ActiveHediff(Pawn p)
    {
        Map map = p.Map;
        if (map?.weatherManager.curWeather != Snowstrom_MiscDefOf.OAGene_IceRain)
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

