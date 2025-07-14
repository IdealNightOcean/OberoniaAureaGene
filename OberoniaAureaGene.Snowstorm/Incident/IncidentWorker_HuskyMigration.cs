using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_HuskyMigration : IncidentWorker
{
    private static readonly IntRange AnimalsCount = new(8, 12);

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        return TryFindStartAndEndCells(map, out _, out _);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!TryFindStartAndEndCells(map, out IntVec3 start, out IntVec3 end))
        {
            return false;
        }
        Rot4 rot = Rot4.FromAngleFlat((map.Center - start).AngleFlat);
        List<Pawn> animals = GenerateAnimals(Snowstorm_RimWorldDefOf.Husky, map.Tile);
        for (int i = 0; i < animals.Count; i++)
        {
            Pawn newThing = animals[i];
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(start, map, 10);
            GenSpawn.Spawn(newThing, loc, map, rot);
        }
        LordMaker.MakeNewLord(null, new LordJob_ExitMapNear(end, LocomotionUrgency.Walk), map, animals);
        SendStandardLetter(parms, animals[0]);
        return true;
    }
    private bool TryFindStartAndEndCells(Map map, out IntVec3 start, out IntVec3 end)
    {
        if (!RCellFinder.TryFindRandomPawnEntryCell(out start, map, CellFinder.EdgeRoadChance_Animal))
        {
            end = IntVec3.Invalid;
            return false;
        }
        end = IntVec3.Invalid;
        for (int i = 0; i < 8; i++)
        {
            IntVec3 startLocal = start;
            if (!CellFinder.TryFindRandomEdgeCellWith((IntVec3 x) => map.reachability.CanReach(startLocal, x, PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors).WithFenceblocked(forceFenceblocked: true)), map, CellFinder.EdgeRoadChance_Ignore, out IntVec3 result))
            {
                break;
            }
            if (!end.IsValid || result.DistanceToSquared(start) > end.DistanceToSquared(start))
            {
                end = result;
            }
        }
        return end.IsValid;
    }

    private List<Pawn> GenerateAnimals(PawnKindDef animalKind, int tile)
    {
        int randomInRange = AnimalsCount.RandomInRange;
        randomInRange = Mathf.Max(randomInRange, Mathf.CeilToInt(4f / animalKind.RaceProps.baseBodySize));
        List<Pawn> animals = [];
        for (int i = 0; i < randomInRange; i++)
        {
            Pawn animal = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animalKind, null, PawnGenerationContext.NonPlayer, tile));
            animals.Add(animal);
        }
        return animals;
    }
}
