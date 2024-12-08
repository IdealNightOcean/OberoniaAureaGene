using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_MakeGameCondition_ForceWorld : IncidentWorker_MakeGameCondition
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        parms.target = Find.World;
        return base.CanFireNowSub(parms);
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        parms.target = Find.World;
        return base.TryExecuteWorker(parms);
    }
}
