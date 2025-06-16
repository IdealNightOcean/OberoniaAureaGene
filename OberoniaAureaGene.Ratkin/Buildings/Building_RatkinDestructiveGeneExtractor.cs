using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class Building_RatkinDestructiveGeneExtractor : Building_GeneExtractorBase
{
    protected override string CommandInsertPersonStr => "OAGene_DestructiveGeneExtractorInsert".Translate();
    protected override string CommandInsertPersonDescStr => "OAGene_DestructiveGeneExtractorInsertDesc".Translate();
    protected override int TicksToExtract => 7500;
    public override void TryStartWork(Pawn pawn)
    {
        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("OAGene_ConfirmRipGenePawn".Translate(pawn.Named("PAWN")), delegate
        {
            StartWork(pawn);
        }, destructive: true));
    }
    protected override void CancelWork()
    {
        if (ContainedPawn is not null)
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("OAGene_ConfirmCancelRipGene".Translate(ContainedPawn.Named("PAWN")), delegate
            {
                KillContainedPawn(ContainedPawn);
                base.CancelWork();
            }, destructive: true));
        }
        else
        {
            base.CancelWork();
        }
    }

    protected override void FinishWork()
    {
        Map map = base.Map;
        Pawn containedPawn = ContainedPawn;
        if (containedPawn is not null)
        {
            List<GeneDef> tempGenes = containedPawn.genes.GenesListForReading.Select(g => g.def).ToList();
            int num = new IntRange(2 + tempGenes.Count / 5, 6).RandomInRange;
            targetGenes.AddRange(tempGenes.InRandomOrder().Take(num));
            foreach (GeneDef gene in targetGenes)
            {
                Genepack genepack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);
                genepack.Initialize([gene]);
                GenPlace.TryPlaceThing(genepack, placePos, map, ThingPlaceMode.Near);
            }
            KillContainedPawn(containedPawn);
        }
        base.FinishWork();
    }

    private static void KillContainedPawn(Pawn pawn)
    {
        pawn.health.AddHediff(HediffDefOf.MissingBodyPart, pawn.health.hediffSet.GetBrain());
        if (!pawn.Dead)
        {
            pawn.Kill(null);
        }
        Messages.Message("OAGene_PawnKilledDestructiveGeneExtract".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.NegativeHealthEvent);
    }
}