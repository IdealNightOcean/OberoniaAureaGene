using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormCultistArrival : IncidentWorker_IsolatedTraderCaravanArrival
{
    protected override IsolatedPawnGroupMakerDef PawnGroupMakerDef => Snowstrom_MiscDefOf.OAGene_GroupMaker_SnowstormCultist;

    protected override bool TryResolveFaction(IncidentParms parms)
    {
        parms.faction ??= OAFrame_FactionUtility.ValidTempFactionsOfDef(FactionDefOf.OutlanderCivil).Where(f => !f.HostileTo(Faction.OfPlayer)).RandomElementWithFallback(null);
        parms.faction ??= OAFrame_FactionUtility.GenerateTempFaction(FactionDefOf.OutlanderCivil);
        parms.faction ??= Find.FactionManager.RandomNonHostileFaction(allowNonHumanlike: false);
        return parms.faction != null;
    }

    protected override List<Pawn> SpawnTradePawns(IncidentParms parms, PawnGroupMakerParms groupMakerParms, PawnGroupMaker groupMaker)
    {
        List<Pawn> pawns = base.SpawnTradePawns(parms, groupMakerParms, groupMaker);
        foreach (Pawn pawn in pawns)
        {
            if (pawn.RaceProps.Humanlike)
            {
                pawn.health.AddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_SnowstormCultist);
            }
        }
        return pawns;
    }
    protected override void SendLetter(IncidentParms parms, List<Pawn> pawns)
    {
        SendStandardLetter(parms, pawns[0]);
    }
}
