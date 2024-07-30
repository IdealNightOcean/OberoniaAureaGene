﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

public class Building_RatkinDestructiveGeneExtractor : Building_GeneExtractorBase
{
    protected override int TicksToExtract => 7500;
    public override void TryStartWork(Pawn pawn)
    {
        StartWork(pawn);
    }

    protected override void FinishWork()
    {
        Map map = base.Map;
        Pawn containedPawn = ContainedPawn;
        if (containedPawn != null)
        {
            List<GeneDef> tempGenes = containedPawn.genes.GenesListForReading.Select(g => g.def).ToList();
            int num = new IntRange(2 + tempGenes.Count / 10, 5).RandomInRange;
            targetGenes.AddRange(tempGenes.InRandomOrder().Take(num));
            foreach (GeneDef gene in targetGenes)
            {
                Genepack genepack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);
                genepack.Initialize([gene]);
                GenPlace.TryPlaceThing(genepack, placePos, map, ThingPlaceMode.Near);
            }
            containedPawn.health.AddHediff(HediffDefOf.MissingBodyPart, containedPawn.health.hediffSet.GetBrain());
            if (!containedPawn.Dead)
            {
                containedPawn.Kill(null);
            }
            Messages.Message("OAGene_PawnKilledDestructiveGeneExtract".Translate(containedPawn.Named("PAWN")), containedPawn, MessageTypeDefOf.NegativeHealthEvent);
        }
        base.FinishWork();
    }
}
