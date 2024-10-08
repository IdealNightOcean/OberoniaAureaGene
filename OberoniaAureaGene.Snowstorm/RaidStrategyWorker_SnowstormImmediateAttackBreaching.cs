﻿using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class RaidStrategyWorker_SnowstormImmediateAttackBreaching : RaidStrategyWorker_ImmediateAttackBreaching
{
    protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
    {
        if (pawns != null)
        {
            foreach (Pawn pawn in pawns)
            {
                pawn.health.AddHediff(Snowstrom_MiscDefOf.OAGene_Hediff_PreparationWarm);
            }
        }
        return base.MakeLordJob(parms, map, pawns, raidSeed);
    }
}
