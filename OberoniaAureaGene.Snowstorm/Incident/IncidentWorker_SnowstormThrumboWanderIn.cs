﻿using OberoniaAurea_Frame.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormThrumboWanderIn : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }

        if (RCellFinder.TryFindRandomPawnEntryCell(out var _, map, CellFinder.EdgeRoadChance_Animal))
        {
            return true;
        }
        return false;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 entryCell, map, CellFinder.EdgeRoadChance_Animal))
        {
            return false;
        }
        SpawnThrumbo(entryCell, map);
        SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new TargetInfo(entryCell, map));
        return true;
    }
    private void SpawnThrumbo(IntVec3 location, Map map)
    {
        IntVec3 loc = CellFinder.RandomClosewalkCellNear(location, map, 12);
        Pawn pawn = PawnGenerator.GeneratePawn(OAFrame_PawnGenerateUtility.CommonPawnGenerationRequest(PawnKindDefOf.Thrumbo));
        GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
        pawn.SetFaction(Faction.OfPlayer);
        pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_SpecialThrumbo);
        Pawn_TrainingTracker trainingTracker = pawn.training;
        if (trainingTracker != null)
        {
            IEnumerable<TrainableDef> trainableDefs = DefDatabase<TrainableDef>.AllDefsListForReading.Where(d => trainingTracker.CanAssignToTrain(d));
            foreach (TrainableDef trainableDef in trainableDefs)
            {
                trainingTracker.SetWantedRecursive(trainableDef, true);
                trainingTracker.Train(trainableDef, null, complete: true);
            }
        }
    }
}
