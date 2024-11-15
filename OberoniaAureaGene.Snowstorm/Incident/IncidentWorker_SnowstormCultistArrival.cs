using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormCultistArrival : IncidentWorker_IsolatedTraderCaravanArrival
{
    protected override IsolatedPawnGroupMakerDef PawnGroupMakerDef => Snowstrom_MiscDefOf.OAGene_GroupMaker_SnowstormCultist;

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
