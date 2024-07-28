using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class Building_OARatkinGeneDiscriminator : Building_GeneExtractorBase
{
    protected override int TicksToExtract => 130000;
    protected static readonly SimpleCurve FailChanceCurve =
    [
        new CurvePoint(1f, 0f),
        new CurvePoint(2f, 0.25f),
        new CurvePoint(3f, 0.4f),
        new CurvePoint(4f, 0.55f)
    ];

    public override void PostMake()
    {
        base.PostMake();
    }

    public override void TryStartWork(Pawn pawn)
    {
        Find.WindowStack.Add(new Dialog_ExtractGenes(pawn, this));
    }
    protected override void FinishWork()
    {
        Map map = base.Map;
        Pawn containedPawn = ContainedPawn;
        bool success = false;
        float faliChance = FailChanceCurve.Evaluate(targetGenes.Count);
        if (!Rand.Chance(faliChance))
        {
            success = true;
            foreach (GeneDef gene in targetGenes)
            {
                Genepack genepack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);
                genepack.Initialize([gene]);
                GenPlace.TryPlaceThing(genepack, placePos, map, ThingPlaceMode.Near);
            }
        }
        GeneUtility.ExtractXenogerm(containedPawn, Mathf.RoundToInt(60000f * GeneTuning.GeneExtractorRegrowingDurationDaysRange.RandomInRange));
        if (!containedPawn.Dead && (containedPawn.IsPrisonerOfColony || containedPawn.IsSlaveOfColony))
        {
            containedPawn.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.XenogermHarvested_Prisoner);
        }
        if (success)
        {
            Messages.Message("GeneExtractionComplete".Translate(containedPawn.Named("PAWN")) + ": " + targetGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), containedPawn, MessageTypeDefOf.PositiveEvent);
        }
        else
        {
            Messages.Message("OAGene_GeneExtractionFail".Translate(containedPawn.Named("PAWN")).CapitalizeFirst(), new LookTargets(containedPawn), MessageTypeDefOf.NegativeEvent);
        }
        base.FinishWork();
    }
}