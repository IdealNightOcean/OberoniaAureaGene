using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class JobDriver_RecallHometown : JobDriver_Skygaze
{
    public override string GetReport()
    {
        return ReportStringProcessed(job.def.reportString);
    }
}
