using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_Drafted : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        return p.Drafted ? ThoughtState.ActiveDefault : ThoughtState.Inactive;
    }
}