using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene.Snowstorm;

public class JobDriver_TakeIceCrystalOutOfCollector : JobDriver
{
    private const TargetIndex CollectorInd = TargetIndex.A;
    private const TargetIndex IceCrystalToHaulInd = TargetIndex.B;
    private const TargetIndex StorageCellInd = TargetIndex.C;

    private const int Duration = 200;

    protected Building_IceCrystalCollector Collector => (Building_IceCrystalCollector)job.GetTarget(CollectorInd).Thing;

    protected Thing Bioferrite => job.GetTarget(IceCrystalToHaulInd).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Collector, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(CollectorInd);
        this.FailOnBurningImmobile(CollectorInd);
        yield return Toils_Goto.GotoThing(CollectorInd, PathEndMode.Touch);
        yield return Toils_General.Wait(Duration).FailOnDestroyedNullOrForbidden(CollectorInd).FailOnCannotTouch(CollectorInd, PathEndMode.Touch)
            .FailOn(() => Collector.CurStorge < 1f)
            .WithProgressBarToilDelay(CollectorInd);
        Toil toil = ToilMaker.MakeToil("MakeNewToils");
        toil.initAction = delegate
        {
            Thing thing = Collector.TakeOutBioferrite();
            GenPlace.TryPlaceThing(thing, pawn.Position, Map, ThingPlaceMode.Near);
            StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
            if (StoreUtility.TryFindBestBetterStoreCellFor(thing, pawn, Map, currentPriority, pawn.Faction, out IntVec3 foundCell))
            {
                job.SetTarget(StorageCellInd, foundCell);
                job.SetTarget(IceCrystalToHaulInd, thing);
                job.count = thing.stackCount;
            }
            else
            {
                EndJobWith(JobCondition.Incompletable);
            }
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        yield return toil;
        yield return Toils_Reserve.Reserve(IceCrystalToHaulInd);
        yield return Toils_Reserve.Reserve(StorageCellInd);
        yield return Toils_Goto.GotoThing(IceCrystalToHaulInd, PathEndMode.ClosestTouch);
        yield return Toils_Haul.StartCarryThing(IceCrystalToHaulInd);
        Toil carryToCell = Toils_Haul.CarryHauledThingToCell(StorageCellInd);
        yield return carryToCell;
        yield return Toils_Haul.PlaceHauledThingInCell(StorageCellInd, carryToCell, storageMode: true);
    }
}

