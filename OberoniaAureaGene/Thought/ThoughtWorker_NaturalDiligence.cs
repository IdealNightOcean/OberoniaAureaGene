using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_IfWorking : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if(p.timetable == null)
        {
            return ThoughtState.Inactive;
        }
        if (p.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
        {
            return ThoughtState.ActiveAtStage(1);
        }
        else
        {
            return ThoughtState.ActiveAtStage(0);
        }
    }
}
