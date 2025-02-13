﻿using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class HediffCompPropertiesp_ColdImmersion : HediffCompProperties
{
    public HediffDef hediffHuman;
    public HediffDef hediffInsectoid;
    public HediffCompPropertiesp_ColdImmersion()
    {
        compClass = typeof(HediffComp_ColdImmersion);
    }
}

[StaticConstructorOnStartup]
public class HediffComp_ColdImmersion : HediffComp
{
    protected HediffDef hediffDef;
    HediffCompPropertiesp_ColdImmersion Props => props as HediffCompPropertiesp_ColdImmersion;
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        hediffDef = (parent.pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid) ? Props.hediffInsectoid : Props.hediffHuman;
    }
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn.IsHashIntervalTick(250))
        {
            CheckHypothermia(parent.pawn, hediffDef, parent.Severity);
        }
    }

    protected static void CheckHypothermia(Pawn pawn, HediffDef hediffDef, float immersionSeverity)
    {
        if (immersionSeverity >= 0.04f && pawn.AmbientTemperature < OAGeneUtility.ComfyTemperatureMin(pawn))
        {
            float hypothermiaIncrease = ((int)(immersionSeverity % 0.1f) + 1) * 0.0008f + 0.003f;
            HealthUtility.AdjustSeverity(pawn, hediffDef, hypothermiaIncrease);
        }
    }
    public override void CompExposeData()
    {
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            hediffDef = (parent.pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid) ? Props.hediffInsectoid : Props.hediffHuman;
        }
    }
}
