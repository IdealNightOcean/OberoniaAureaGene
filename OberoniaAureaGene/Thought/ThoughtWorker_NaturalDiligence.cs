using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_IfWorking : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p.timetable == null || p.timetable.CurrentAssignment != TimeAssignmentDefOf.Work)
        {
            return ThoughtState.ActiveAtStage(0);
        }
        else
        {
            return ThoughtState.ActiveAtStage(1);
        }
    }
}
