using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_AfterSnowstormTraderCaravanArrival : IncidentWorker_TraderCaravanArrival
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!TryResolveParms(parms))
        {
            return false;
        }
        if (parms.faction.HostileTo(Faction.OfPlayer))
        {
            return false;
        }
        Map map = (Map)parms.target;
        List<Pawn> pawns = SpawnPawns(parms);
        if (pawns.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < pawns.Count; i++)
        {
            Pawn pawn1 = pawns[i];
            Snowstorm_MiscUtility.SetColdPreparation(pawn1, Snowstorm_HediffDefOf.OAGene_Hediff_ColdPreparation_Neutral);
            if (pawn1.needs != null && pawn1.needs.food != null)
            {
                pawn1.needs.food.CurLevel = pawn1.needs.food.MaxLevel;
            }
        }
        TraderKindDef traderKind = null;
        for (int j = 0; j < pawns.Count; j++)
        {
            Pawn pawn2 = pawns[j];
            if (pawn2.TraderKind != null)
            {
                traderKind = pawn2.TraderKind;
                break;
            }
        }
        SendLetter(parms, pawns, traderKind);
        RCellFinder.TryFindRandomSpotJustOutsideColony(pawns[0].Position, pawns[0].MapHeld, pawns[0], out var result, delegate (IntVec3 c)
        {
            for (int k = 0; k < pawns.Count; k++)
            {
                if (!pawns[k].CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                {
                    return false;
                }
            }
            return true;
        });
        LordJob_TradeWithColony lordJob = new(parms.faction, result);
        LordMaker.MakeNewLord(parms.faction, lordJob, map, pawns);
        return true;
    }
}