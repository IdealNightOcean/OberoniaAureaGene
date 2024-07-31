﻿using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class Gene_BloodCellsAutophagy : Gene
{
    public override void Tick()
    {
        if (pawn.IsHashIntervalTick(2500))
        {
            CheckFood(pawn);
        }
    }
    public static void CheckFood(Pawn pawn)
    {
        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition);
        if (hediff != null && hediff.Severity > 0.05)
        {
            OAGeneUtility.AdjustOrAddHediff(pawn, OberoniaAureaGeneDefOf.OAGene_BloodCellsAutophagy, overrideDisappearTicks: 30000);
        }
    }
}