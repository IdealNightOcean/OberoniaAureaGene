using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class Building_OAGeneFragmentInterceptor : Building_GeneExtractorBase
{
    protected static readonly IntRange ExtractedGeneorCountRange = new(2, 4);
    protected static readonly IntRange GeneExtractorRegrowingDurationDaysRange = new(4, 8);
    public override void TryStartWork(Pawn pawn)
    {
        StartWork(pawn);
    }
    protected override void FinishWork()
    {
        Map map = base.Map;
        Pawn containedPawn = ContainedPawn;
        if (containedPawn is not null)
        {
            int num = ExtractedGeneorCountRange.RandomInRange;
            targetGenes.AddRange(containedPawn.genes.GenesListForReading.Where(g => g.def.biostatArc <= 0).Select(ng => ng.def).InRandomOrder().Take(num));
            Genepack genepack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);
            genepack.Initialize(targetGenes);
            GenPlace.TryPlaceThing(genepack, placePos, map, ThingPlaceMode.Near);
            GeneUtility.ExtractXenogerm(containedPawn, Mathf.RoundToInt(60000f * GeneExtractorRegrowingDurationDaysRange.RandomInRange));
            HediffComp_Disappears disappearsComp = containedPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermLossShock)?.TryGetComp<HediffComp_Disappears>();
            if (disappearsComp is not null)
            {
                disappearsComp.ticksToDisappear = 30000;
            }
            Messages.Message("GeneExtractionComplete".Translate(containedPawn.Named("PAWN")) + ": " + targetGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), new LookTargets(containedPawn, genepack), MessageTypeDefOf.PositiveEvent);
        }
        base.FinishWork();
    }
}
