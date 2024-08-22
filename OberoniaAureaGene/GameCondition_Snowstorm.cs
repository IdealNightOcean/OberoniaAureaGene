using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class GameCondition_Snowstorm : GameCondition_ForceWeather
{
    private int snowWeatherChangeTick;
    public override void GameConditionTick()
    {
        snowWeatherChangeTick--;
        if (snowWeatherChangeTick <= 0)
        {
            weather = Rand.Chance(0.7f) ? OAGene_RimWorldDefOf.SnowGentle : OAGene_RimWorldDefOf.SnowHard;
            snowWeatherChangeTick = 60000;
        }
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref snowWeatherChangeTick, "snowWeatherChangeTick", 0);
    }
}
