﻿using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class GameCondition_TemperatureChange : GameCondition
{
    public float tempOffset;

    public override int TransitionTicks => 6000;

    public override void Init()
    {
        base.Init();
        tempOffset = def.temperatureOffset;
    }

    public override float TemperatureOffset()
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks, tempOffset);
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
    }
}
