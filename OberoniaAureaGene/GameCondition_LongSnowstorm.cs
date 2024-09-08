using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class GameCondition_LongSnowstorm : GameCondition
{
    private static readonly float SkyGlow = 0.9f;

    public float tempOffset;
    public override int TransitionTicks => 5000;

    public override void Init()
    {
        base.Init();
        tempOffset = def.temperatureOffset;
    }

    public override float TemperatureOffset()
    {
        return tempOffset;
    }
    private int snowWeatherChangeTick;

    public override SkyTarget? SkyTarget(Map map)
    {
        return new(SkyGlow, default, 1f, 1f);
    }
    public override float SkyTargetLerpFactor(Map map)
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref snowWeatherChangeTick, "snowWeatherChangeTick", 0);
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
    }
}
