using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

public class GameCondition_SnowstormBase : GameCondition_ForceWeather
{
    protected static readonly float SkyGlow = 0.10f;
    protected static SkyColorSet SnowstormSkyColors = new(new Color(0.416f, 0.553f, 0.643f), new Color(0.92f, 0.92f, 0.92f), new Color(0.6f, 0.6f, 0.6f), 0.9f);

    public float tempOffset;

    public override int TransitionTicks => 5000;
    private readonly List<SkyOverlay> snowHardOverlay = [new WeatherOverlay_SnowExtreme()];

    public override void Init()
    {
        base.Init();
        tempOffset = def.temperatureOffset;
        weather = def.weatherDef;
    }
    public override float TemperatureOffset()
    {
        return tempOffset;
    }
    public override SkyTarget? SkyTarget(Map map)
    {
        return new(SkyGlow, SnowstormSkyColors, 1f, 1f);
    }
    public override float SkyTargetLerpFactor(Map map)
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
    }
    public override List<SkyOverlay> SkyOverlays(Map map)
    {
        return snowHardOverlay;
    }
    public override void GameConditionDraw(Map map)
    {
        for (int i = 0; i < snowHardOverlay.Count; i++)
        {
            snowHardOverlay[i].DrawOverlay(map);
        }
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
    }
}
