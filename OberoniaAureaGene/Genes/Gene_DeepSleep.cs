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
        if (pawn.CurJob == null || pawn.jobs.curDriver == null)
        {
            return;
        }
        if (pawn.jobs.curDriver.asleep)
        {
            OAGeneUtility.AdjustOrAddHediff(pawn, OberoniaAureaGeneDefOf.OAGene_DeepSleep, overrideDisappearTicks: 750);
        }
    }
}
