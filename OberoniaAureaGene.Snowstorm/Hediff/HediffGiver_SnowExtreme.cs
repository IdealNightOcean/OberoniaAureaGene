using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class HediffGiver_SnowExtreme : HediffGiver_SnowExtremeBase
{
    public HediffDef iceStormHediff;

    //冰晶暴风雪
    protected override void IceStormActive(Pawn pawn)
    {
        if (SnowstormUtility.IsIceStormWeather(pawn.Map))
        {
            pawn.health.AddHediff(iceStormHediff);
        }
    }

}