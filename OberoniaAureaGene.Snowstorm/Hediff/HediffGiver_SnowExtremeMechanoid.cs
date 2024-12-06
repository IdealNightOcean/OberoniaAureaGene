using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowExtremeMechanoid : HediffGiver_SnowExtremeMechanoidBase
{
    public HediffDef iceStormHediff;
    public HediffDef smallBodyHediff;
    protected override void TryActiveHediff(Pawn pawn)
    {
        base.TryActiveHediff(pawn);
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
