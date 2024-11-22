﻿using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class Hediff_SnowExtremePlayerHidden : HediffWithComps
{
    public bool CanGetHediffNow => !obliviousGived && Find.TickManager.TicksGame > nextGetHediffTick;
    public int nextGetHediffTick = -1;
    public bool obliviousGived;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref nextGetHediffTick, "nextGetHediffTick", 0);
        Scribe_Values.Look(ref obliviousGived, "obliviousGived", defaultValue: false);
    }
}
