using OberoniaAurea_Frame;
using OberoniaAurea_Frame.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_IsolatedTraderCaravanArrival : IncidentWorker_NeutralGroup
{
    protected virtual IsolatedPawnGroupMakerDef PawnGroupMakerDef => null;
    protected override PawnGroupKindDef PawnGroupKindDef => PawnGroupKindDefOf.Trader;

    protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
    {
        return true;
    }
    protected override void ResolveParmsPoints(IncidentParms parms)
    {
        parms.points = TraderCaravanUtility.GenerateGuardPoints();
    }
    protected override bool TryResolveParmsGeneral(IncidentParms parms)
    {
        if (PawnGroupMakerDef == null)
        {
            return false;
        }
        Map map = (Map)parms.target;
        if (!parms.spawnCenter.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out parms.spawnCenter, map, CellFinder.EdgeRoadChance_Neutral))
        {
            return false;
        }
        parms.faction ??= OberoniaAureaFrameUtility.GenerateTempFaction(FactionDefOf.OutlanderCivil);
        if (parms.faction == null)
        {
            return false;
        }

        parms.traderKind ??= PawnGroupMakerDef.traderKind;
        if (parms.traderKind == null)
        {
            return false;
        }

        return true;
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!TryResolveParms(parms))
        {
            return false;
        }
        if (parms.faction.HostileTo(Faction.OfPlayer))
        {
            return false;
        }
        PawnGroupMakerParms groupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDef, parms, ensureCanGenerateAtLeastOnePawn: true);
        if (!PawnGenerateUtility.TryGetRandomPawnGroupMaker(groupMakerParms, PawnGroupMakerDef, out PawnGroupMaker groupMaker))
        {
            return false;
        }
        List<Pawn> pawns = SpawnTradePawns(parms, groupMakerParms, groupMaker);
        if (pawns.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i].needs != null && pawns[i].needs.food != null)
            {
                pawns[i].needs.food.CurLevel = pawns[i].needs.food.MaxLevel;
            }
        }
        SendLetter(parms, pawns);
        RCellFinder.TryFindRandomSpotJustOutsideColony(pawns[0].Position, pawns[0].MapHeld, pawns[0], out IntVec3 result, delegate (IntVec3 c)
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
    protected virtual List<Pawn> SpawnTradePawns(IncidentParms parms, PawnGroupMakerParms groupMakerParms, PawnGroupMaker groupMaker)
    {
        Map map = (Map)parms.target;
        List<Pawn> pawns = PawnGenerateUtility.GeneratePawns(groupMakerParms, groupMaker, needFaction: false, warnOnZeroResults: false).ToList();
        foreach (Pawn pawn in pawns)
        {
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, map, 5);
            GenSpawn.Spawn(pawn, loc, map);
            parms.storeGeneratedNeutralPawns?.Add(pawn);
        }
        return pawns;
    }


    protected virtual void SendLetter(IncidentParms parms, List<Pawn> pawns)
    {
        TaggedString letterLabel = "LetterLabelTraderCaravanArrival".Translate(parms.faction.Name, parms.traderKind.label).CapitalizeFirst();
        TaggedString letterText = "LetterTraderCaravanArrival".Translate(parms.faction.NameColored, parms.traderKind.label).CapitalizeFirst();
        letterText += "\n\n" + "LetterCaravanArrivalCommonWarning".Translate();
        PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(pawns, ref letterLabel, ref letterText, "LetterRelatedPawnsNeutralGroup".Translate(Faction.OfPlayer.def.pawnsPlural), informEvenIfSeenBefore: true);
        SendStandardLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, parms, pawns[0]);
    }

}
