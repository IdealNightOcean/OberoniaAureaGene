using OberoniaAurea_Frame;
using RimWorld;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormSnowstormCultistArrival : IncidentWorker_TraderCaravanNoFactionArrival
{
    protected override PawnGroupKindDef PawnGroupKindDef => Snowstrom_MiscDefOf.OAGene_GroupKind_SnowstormCultist;
    protected override IsolatedPawnGroupMakerDef PawnGroupMakerDef => Snowstrom_MiscDefOf.OAGene_GroupMaker_SnowstormCultist;
    protected override bool TryResolveParmsGeneral(IncidentParms parms)
    {
        parms.traderKind = Snowstrom_MiscDefOf.Caravan_Neolithic_BulkGoods;
        return base.TryResolveParmsGeneral(parms);
    }
}
