using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowExtremeMechanoid : HediffGiver_SnowExtremeMechanoidBase
{
    public HediffDef iceStormHediff;
    protected override void TryActiveHediff(Pawn pawn)
    {
        base.TryActiveHediff(pawn);
        if (pawn.Map?.weatherManager.curWeather == Snowstrom_MiscDefOf.OAGene_IceSnowExtreme)
        {
            pawn.health.AddHediff(iceStormHediff);
        }
    }
}
