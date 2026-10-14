using OberoniaAurea_Frame;
using OberoniaAurea_Frame.Utility;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class Gene_BloodCellsAutophagy : Gene
{
    private static readonly HediffGiveParams hediffGiveParams = new()
    {
        OverrideDisappearTicks = 30000
    };

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
        if (hediff is not null && hediff.Severity > 0.05f)
        {
            hediffGiveParams.HediffToGive = OAGene_HediffDefOf.OAGene_BloodCellsAutophagy;
            pawn.GetOrAddHediff(hediffGiveParams);
        }
    }
}
