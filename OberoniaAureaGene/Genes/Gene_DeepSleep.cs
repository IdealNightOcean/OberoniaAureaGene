using OberoniaAurea_Frame;
using Verse;

namespace OberoniaAureaGene;

public class Gene_DeepSleep : Gene
{
    public override void Tick()
    {
        if (pawn.IsHashIntervalTick(600))
        {
            CheckAlseep(pawn);
        }
    }
    private static void CheckAlseep(Pawn pawn)
    {
        if (pawn.CurJob is null || pawn.jobs.curDriver is null)
        {
            return;
        }
        if (pawn.jobs.curDriver.asleep)
        {
            OAFrame_PawnUtility.AdjustOrAddHediff(pawn, OAGene_HediffDefOf.OAGene_DeepSleep, overrideDisappearTicks: 750);
        }
    }
}
