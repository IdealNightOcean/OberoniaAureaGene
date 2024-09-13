using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class GameCondition_LongSnowstorm : GameCondition
{
    private static readonly float SkyGlow = 0.9f;
    private static readonly Color LongColor = new(0.92f, 0.92f, 0.92f);
    private static SkyColorSet LongSnowSkyColors = new(LongColor, LongColor, LongColor, 1f);


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
        return new(SkyGlow, LongSnowSkyColors, 1f, 1f);
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
