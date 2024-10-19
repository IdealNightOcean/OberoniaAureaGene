﻿using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffGiver_SnowExtreme : HediffGiver_SnowExtremeBase
{
    public HediffDef iceStormHediff;
    protected override void TryActiveHediff(Pawn pawn, float ambientTemperature)
    {
        base.TryActiveHediff(pawn, ambientTemperature);
        if (pawn.Map?.weatherManager.curWeather == Snowstrom_MiscDefOf.OAGene_IceSnowExtreme)
        {
            pawn.health.AddHediff(iceStormHediff);
        }
    }
}
