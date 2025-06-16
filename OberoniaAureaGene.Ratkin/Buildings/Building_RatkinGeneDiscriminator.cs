using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
public class Building_RatkinGeneDiscriminator : Building_GeneDiscriminatorBase
{
    protected override int TicksToDiscriminat => 180000;
    protected static readonly SimpleCurve FailChanceCurve =
    [
        new CurvePoint(1f, 0f),
        new CurvePoint(2f, 0.4f),
        new CurvePoint(3f, 0.6f),
        new CurvePoint(4f, 0.8f)
    ];

    protected override void FinishWork()
    {
        Map map = base.Map;
        Genepack containedGenepack = ContainedGenepack;
        bool success = false;
        float faliChance = FailChanceCurve.Evaluate(containedGenepack.GeneSet.GenesListForReading.Count);
        if (!Rand.Chance(faliChance))
        {
            success = true;
            Genepack genepack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);
            genepack.Initialize([targetGeneDef]);
            GenPlace.TryPlaceThing(genepack, placePos, map, ThingPlaceMode.Near);

        }
        if (success)
        {
            Messages.Message("OAGene_GeneDiscriminationComplete".Translate(targetGeneDef.label).CapitalizeFirst(), targetGenepack, MessageTypeDefOf.PositiveEvent);
        }
        else
        {
            Messages.Message("OAGene_GeneDiscriminationFail".Translate(targetGeneDef.label).CapitalizeFirst(), targetGenepack, MessageTypeDefOf.NegativeEvent);
        }
        base.FinishWork();
    }
}