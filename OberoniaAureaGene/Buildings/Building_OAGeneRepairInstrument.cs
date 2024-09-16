using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class Building_OAGeneRepairInstrument : Building_EnterableBase
{
    protected override int AllTicks => 10000;
    protected int searchTicksRemaining;
    protected bool autoSelect;
    protected static readonly int SearchTicks = 1000;
    public override AcceptanceReport CanAcceptPawn(Pawn pawn)
    {
        AcceptanceReport baseReport = base.CanAcceptPawn(pawn);
        if (!baseReport.Accepted)
        {
            return baseReport;
        }
        if (!pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
        {
            return false;
        }
        if (pawn.health.hediffSet.HasHediff(OAGene_HediffDefOf.OAGene_XenogermRepairing))
        {
            return "OAGene_XenogermRepairing".Translate();
        }
        return true;
    }
    public override void Tick()
    {
        base.Tick();
        searchTicksRemaining--;
        if (searchTicksRemaining < 0)
        {
            TryAutoStartNewWork();
            searchTicksRemaining = SearchTicks;
        }
    }
    protected void TryAutoStartNewWork()
    {
        if (!autoSelect || base.Working || selectedPawn != null || !PowerOn)
        {
            return;
        }
        foreach (Pawn pawn in base.Map.mapPawns.FreeColonistsAndPrisonersSpawned)
        {
            AcceptanceReport acceptanceReport = CanAcceptPawn(pawn);
            if (acceptanceReport.Accepted)
            {
                TryStartWork(pawn);
                return;
            }
        }
    }
    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (Gizmo gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }
        Command_Action command_ContinueWork = new()
        {
            defaultLabel = "OAGene_CommandAutoSelect".Translate(),
            defaultDesc = "OAGene_CommandAutoSelectDesc".Translate(this.Label),
            icon = autoSelect ? IconUtility.CommandContinueWorkTrue : IconUtility.CommandContinueWorkFalse,
            action = delegate
            {
                autoSelect = !autoSelect;
            },
            activateSound = SoundDefOf.Tick_Tiny
        };
        yield return command_ContinueWork;
    }
    protected override void FinishWork()
    {
        Pawn containedPawn = ContainedPawn;
        if (containedPawn != null)
        {
            HediffComp_Disappears disappearsComp = containedPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating)?.TryGetComp<HediffComp_Disappears>();
            if (disappearsComp != null)
            {
                int num = Mathf.Max((int)(disappearsComp.ticksToDisappear * 0.2f), 120000);
                disappearsComp.ticksToDisappear -= num;
            }
            Hediff hediff = containedPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermLossShock);
            if (hediff != null)
            {
                containedPawn.health.RemoveHediff(hediff);
            }
            containedPawn.health.AddHediff(OAGene_HediffDefOf.OAGene_XenogermRepairing);
        }
        base.FinishWork();
    }

}
