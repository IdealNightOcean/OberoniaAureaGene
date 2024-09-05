using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class GameCondition_LongSnowstorm : GameCondition
{
    public float tempOffset;

    private static readonly float SkyGlow = 0.9f;
    private static SkyTarget SnowstormSky = new(SkyGlow, default, 1f, 1f);
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
        return SnowstormSky;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref snowWeatherChangeTick, "snowWeatherChangeTick", 0);
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
    }
}
