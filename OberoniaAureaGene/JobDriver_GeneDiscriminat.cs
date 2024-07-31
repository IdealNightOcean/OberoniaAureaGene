using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace OberoniaAureaGene;

public class JobDriver_GeneDiscriminat : JobDriver
{
    //A：基因库   B：辨析仪   C：基因组
    private Building_GeneDiscriminatorBase GeneDiscriminat => job.GetTarget(TargetIndex.B).Thing as Building_GeneDiscriminatorBase;
    private CompGeneDiscriminat ContainerComp => GeneDiscriminat.TryGetComp<CompGeneDiscriminat>();
    private Genepack Genepack => (Genepack)job.GetTarget(TargetIndex.C).Thing;
    protected virtual PathEndMode ContainerPathEndMode
    {
        get
        {
            if (!job.GetTarget(TargetIndex.A).Thing.def.hasInteractionCell)
            {
                return PathEndMode.Touch;
            }
            return PathEndMode.InteractionCell;
        }
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        Pawn p = pawn;
        LocalTargetInfo target = job.GetTarget(TargetIndex.A);
        Job obj = job;
        bool errorOnFailed2 = errorOnFailed;
        if (p.Reserve(target, obj, 1, -1, job.def.containerReservationLayer, errorOnFailed2))
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.B), job, 1, -1, null, errorOnFailed);
        }
        return false;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, ContainerPathEndMode).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A)
            .FailOn(() => job.GetTarget(TargetIndex.A).Thing.TryGetComp(out CompGenepackContainer comp) && comp.ContainedGenepacks.Count == 0);
        yield return Toils_General.WaitWhileExtractingContents(TargetIndex.A, TargetIndex.C, 120);
        yield return Toils_General.Do(delegate
        {
            base.TargetThingA.TryGetInnerInteractableThingOwner().TryDrop(TargetThingC, pawn.Position, pawn.Map, ThingPlaceMode.Near, 1, out Thing dropPack);
        });
        yield return Toils_Reserve.Reserve(TargetIndex.C);
        this.FailOn(delegate
        {
            CompGeneDiscriminat containerComp = ContainerComp;
            if (containerComp == null || containerComp.Full)
            {
                return true;
            }
            return !containerComp.leftToLoad.Contains(Genepack) || Genepack.targetContainer != GeneDiscriminat;
        });
        yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.C).FailOnSomeonePhysicallyInteracting(TargetIndex.C);
        yield return Toils_Haul.StartCarryThing(TargetIndex.C, putRemainderInQueue: false, subtractNumTakenFromJobCount: true);
        yield return Toils_Goto.Goto(TargetIndex.B, PathEndMode.Touch);
        Toil toil = Toils_General.Wait(30, TargetIndex.B).WithProgressBarToilDelay(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.B);
        toil.handlingFacing = true;
        yield return toil;
        yield return Toils_Haul.DepositHauledThingInContainer(TargetIndex.B, TargetIndex.C, delegate
        {
            Genepack.def.soundDrop.PlayOneShot(SoundInfo.InMap(GeneDiscriminat));
            Genepack.targetContainer = null;
            CompGeneDiscriminat containerComp2 = ContainerComp;
            containerComp2.leftToLoad.Remove(Genepack);
            GeneDiscriminat.Notify_GenepackLoaded();
            MoteMaker.ThrowText(GeneDiscriminat.DrawPos, pawn.Map, "InsertedThing".Translate($"{containerComp2.innerContainer.Count} / {containerComp2.Props.maxCapacity}"));
        });
    }

}
