using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_SnowyCrystalTreeCooler : GameCondition
{
    private const float TempChangePerTick = 0.001f;

    private const float DefaultTargetTemp = -30f;

    private float tempOffset;

    public float targetTemp = -30f;

    private int SnowyCrystalTreeCount => Snowstorm_MiscUtility.SnowstormMapComp(SingleMap)?.SnowyCrystalTreeCount ?? 0;
    private int TargetTempOffset => SnowyCrystalTreeCount * Comp_SnowyCrystalTree.PreTreeTempCooler;


    public override int TransitionTicks => 2500;

    public override void PostMake()
    {
        base.PostMake();
        tempOffset = 0f;
    }

    public override float TemperatureOffset()
    {
        if (!Permanent)
        {
            return Mathf.Lerp(0f, tempOffset, Mathf.Min(1f, TicksLeft / (float)TransitionTicks));
        }
        return tempOffset;
    }

    public override void GameConditionTick()
    {
        tempOffset += Mathf.Sign(TargetTempOffset - tempOffset) * TempChangePerTick;
        if (SnowyCrystalTreeCount <= 0)
        {
            Permanent = false;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
        Scribe_Values.Look(ref targetTemp, "targetTemp", 20f);
    }
}
