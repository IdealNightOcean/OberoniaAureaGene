using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_BloodLoss : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        Hediff hediff = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
        if (hediff is null)
        {
            return ThoughtState.Inactive;
        }
        return ThoughtState.ActiveAtStage(hediff.CurStageIndex);
    }
}