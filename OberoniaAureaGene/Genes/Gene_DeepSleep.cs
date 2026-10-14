using OberoniaAurea_Frame;
using OberoniaAurea_Frame.Utility;
using Verse;

namespace OberoniaAureaGene;

public class Gene_DeepSleep : Gene
{
    private static readonly HediffGiveParams hediffGiveParams = new()
    {
        OverrideDisappearTicks = 750
    };

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
            hediffGiveParams.HediffToGive = OAGene_HediffDefOf.OAGene_DeepSleep;
            pawn.GetOrAddHediff(hediffGiveParams);
        }
    }
}
