using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowExtreme : HediffGiver_SnowExtremeBase
{
    public HediffDef iceStormHediff;
    public HediffDef smallBodyHediff;
    protected override void TryActiveHediff(Pawn pawn, float ambientTemperature)
    {
        base.TryActiveHediff(pawn, ambientTemperature);
        if (pawn.RaceProps.baseBodySize <= 0.7f)
        {
            pawn.health.AddHediff(smallBodyHediff);
        }
        if (pawn.Map?.weatherManager.curWeather == Snowstorm_MiscDefOf.OAGene_IceSnowExtreme)
        {
            pawn.health.AddHediff(iceStormHediff);
        }
    }
}
