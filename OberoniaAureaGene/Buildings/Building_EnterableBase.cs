using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace OberoniaAureaGene;

public abstract class Building_EnterableBase : Building_Enterable, IThingHolderWithDrawnPawn, IThingHolder
{
    protected int ticksRemaining;
    protected int powerCutTicks;
    protected int allTicks;

    protected CompPowerTrader compPower;
    protected IntVec3 placePos;

    [Unsaved(false)]
    protected Sustainer sustainerWorking;

    [Unsaved(false)]
    protected Effecter progressBar;


    protected static int NoPowerEjectCumulativeTicks = 60000;
    protected virtual int AllTicks => 0;

    protected Pawn ContainedPawn => innerContainer.FirstOrDefault() as Pawn;
    public bool PowerOn => compPower.PowerOn;

    public override bool IsContentsSuspended => false;
    public float HeldPawnDrawPos_Y => DrawPos.y + 1f / 26f;
    public float HeldPawnBodyAngle => base.Rotation.Opposite.AsAngle;
    public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;
    public override Vector3 PawnDrawOffset => IntVec3.West.RotatedBy(base.Rotation).ToVector3() / def.size.x;

    protected virtual string CommandInsertPersonStr => "OAGene_InsertPerson".Translate();
    protected virtual string CommandInsertPersonDescStr => "OAGene_InsertPersonDesc".Translate();

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        compPower = this.TryGetComp<CompPowerTrader>();
        placePos = base.def.hasInteractionCell ? InteractionCell : base.Position;
    }
    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        CancelWork();
        base.DeSpawn(mode);
    }

    public override void Tick()
    {
        base.Tick();
        innerContainer.ThingOwnerTick();
        if (this.IsHashIntervalTick(250))
        {
            compPower.PowerOutput = (base.Working ? (0f - base.PowerComp.Props.PowerConsumption) : (0f - base.PowerComp.Props.idlePowerDraw));
        }
        if (base.Working)
        {
            if (ContainedPawn == null)
            {
                CancelWork();
                return;
            }
            if (PowerOn)
            {
                TickEffects();
                ticksRemaining--;
                if (ticksRemaining <= 0)
                {
                    FinishWork();
                }
                return;
            }
            else
            {
                powerCutTicks++;
                if (powerCutTicks >= 60000)
                {
                    Pawn containedPawn = ContainedPawn;
                    if (containedPawn != null)
                    {
                        Messages.Message("GeneExtractorNoPowerEjectedMessage".Translate(containedPawn.Named("PAWN")), containedPawn, MessageTypeDefOf.NegativeEvent, historical: false);
                    }
                    CancelWork();
                    return;
                }
            }
        }
        else if (selectedPawn != null && selectedPawn.Dead)
        {
            CancelWork();
        }
    }
    protected void TickEffects()
    {
        if (sustainerWorking == null || sustainerWorking.Ended)
        {
            sustainerWorking = SoundDefOf.GeneExtractor_Working.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
        }
        else
        {
            sustainerWorking.Maintain();
        }
        progressBar ??= EffecterDefOf.ProgressBarAlwaysVisible.Spawn();
        progressBar.EffectTick(new TargetInfo(base.Position + IntVec3.North.RotatedBy(base.Rotation), base.Map), TargetInfo.Invalid);
        MoteProgressBar mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
        if (mote != null)
        {
            mote.progress = 1f - Mathf.Clamp01((float)ticksRemaining / allTicks);
            mote.offsetZ = ((base.Rotation == Rot4.North) ? 0.5f : (-0.5f));
        }
    }
    protected virtual void ClearEffects() //清理特效
    {
        progressBar?.Cleanup();
        progressBar = null;
    }

    public override AcceptanceReport CanAcceptPawn(Pawn pawn)
    {
        if (!pawn.IsColonist && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony && (!pawn.IsColonyMutant || !pawn.IsGhoul))
        {
            return false;
        }
        if (selectedPawn != null && selectedPawn != pawn)
        {
            return false;
        }
        if (!pawn.RaceProps.Humanlike || pawn.IsQuestLodger())
        {
            return false;
        }
        if (!PowerOn)
        {
            return "NoPower".Translate().CapitalizeFirst();
        }
        if (innerContainer.Count > 0)
        {
            return "Occupied".Translate();
        }
        return true;
    }
    public virtual void TryStartWork(Pawn pawn)
    {
        allTicks = AllTicks;
        base.SelectPawn(pawn);
    }
    protected virtual void CancelWork()
    {
        startTick = -1;
        selectedPawn = null;
        sustainerWorking = null;
        ClearEffects();
        powerCutTicks = 0;
        innerContainer.TryDropAll(def.hasInteractionCell ? InteractionCell : base.Position, base.Map, ThingPlaceMode.Near);
    }

    protected virtual void FinishWork()
    {
        innerContainer.TryDropAll(placePos, base.Map, ThingPlaceMode.Near);
        selectedPawn = null;
        sustainerWorking = null;
        ClearEffects();
        powerCutTicks = 0;
        startTick = -1;
    }

    public override void TryAcceptPawn(Pawn pawn)
    {
        if ((bool)CanAcceptPawn(pawn))
        {
            selectedPawn = pawn;
            bool num = pawn.DeSpawnOrDeselect();
            if (innerContainer.TryAddOrTransfer(pawn))
            {
                startTick = Find.TickManager.TicksGame;
                ticksRemaining = allTicks;
            }
            if (num)
            {
                Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);
            }
        }
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
    {
        foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
        {
            yield return floatMenuOption;
        }
        if (!selPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly))
        {
            yield return new FloatMenuOption("CannotEnterBuilding".Translate(this) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            yield break;
        }
        FloatMenuOption fOpt = FloatMenuOption_InsertPerson(selPawn);
        if (fOpt != null)
        {
            yield return fOpt;
        }
    }
    protected virtual FloatMenuOption FloatMenuOption_InsertPerson(Pawn selPawn)
    {
        AcceptanceReport acceptanceReport = CanAcceptPawn(selPawn);
        if (acceptanceReport.Accepted)
        {
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("EnterBuilding".Translate(this), delegate
            {
                TryStartWork(selPawn);
            }), selPawn, this);
        }
        else if (base.SelectedPawn == selPawn && !selPawn.IsPrisonerOfColony)
        {
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("EnterBuilding".Translate(this), delegate
            {
                TryStartWork(selPawn);
            }), selPawn, this);
        }
        else if (!acceptanceReport.Reason.NullOrEmpty())
        {
            return new FloatMenuOption("CannotEnterBuilding".Translate(this) + ": " + acceptanceReport.Reason.CapitalizeFirst(), null);
        }
        return null;
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (Gizmo gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }
        if (base.Working)
        {
            Command_Action command_Action = new()
            {
                defaultLabel = "OAGene_CommandCancelWork".Translate(),
                defaultDesc = "OAGene_CommandCancelWorkDesc".Translate(),
                icon = IconUtility.CancelIcon,
                action = CancelWork,
                activateSound = SoundDefOf.Designate_Cancel
            };
            yield return command_Action;
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_Action2 = new()
                {
                    defaultLabel = "DEV: Finish Work",
                    action = delegate
                    {
                        ticksRemaining = 0;
                    }
                };
                yield return command_Action2;
            }
            yield break;
        }
        if (selectedPawn != null)
        {
            Command_Action command_Action3 = new()
            {
                defaultLabel = "OAGene_CommandCancelLoad".Translate(),
                defaultDesc = "OAGene_CommandCancelLoadDesc".Translate(),
                icon = IconUtility.CancelIcon,
                activateSound = SoundDefOf.Designate_Cancel,
                action = delegate
                {
                    if (selectedPawn.CurJobDef == JobDefOf.EnterBuilding)
                    {
                        selectedPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                    CancelWork();
                }
            };
            yield return command_Action3;
            yield break;
        }
        Command_Action command_Action4 = new()
        {
            defaultLabel = "OAGene_InsertPerson".Translate(),
            defaultDesc = "OAGene_InsertPersonDesc".Translate(),
            icon = IconUtility.InsertPawnIcon,
            action = FloatMenu_InsertPerson,
        };
        if (!PowerOn)
        {
            command_Action4.Disable("NoPower".Translate().CapitalizeFirst());
        }
        yield return command_Action4;
    }
    protected virtual void FloatMenu_InsertPerson()
    {
        List<FloatMenuOption> list = [];
        foreach (Pawn pawn in base.Map.mapPawns.AllPawnsSpawned)
        {
            AcceptanceReport acceptanceReport = CanAcceptPawn(pawn);
            if (!acceptanceReport.Accepted)
            {
                if (!acceptanceReport.Reason.NullOrEmpty())
                {
                    list.Add(new FloatMenuOption(pawn.LabelShortCap + ": " + acceptanceReport.Reason, null, pawn, Color.white));
                }
            }
            else
            {
                list.Add(new FloatMenuOption(pawn.LabelShortCap, delegate
                {
                    TryStartWork(pawn);
                }, pawn, Color.white));
            }
        }
        if (!list.Any())
        {
            list.Add(new FloatMenuOption("NoExtractablePawns".Translate(), null));
        }
        Find.WindowStack.Add(new FloatMenu(list));

    }

    public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
    {
        base.DynamicDrawPhaseAt(phase, drawLoc, flip);
        if (base.Working && ContainedPawn != null)
        {
            ContainedPawn.Drawer.renderer.DynamicDrawPhaseAt(phase, drawLoc + PawnDrawOffset, null, neverAimWeapon: true);
        }
    }

    public override string GetInspectString()
    {
        StringBuilder InspectString = new(base.GetInspectString());
        InspectString.AppendInNewLine(PostGetInspectString());
        return InspectString.ToString();
    }
    protected virtual string PostGetInspectString()
    {
        StringBuilder text = new();
        if (selectedPawn != null && innerContainer.Count == 0)
        {
            text.AppendInNewLine("WaitingForPawn".Translate(selectedPawn.Named("PAWN")).Resolve());
        }
        else if (base.Working && ContainedPawn != null)
        {
            if (PowerOn)
            {
                text.AppendInNewLine("DurationLeft".Translate(ticksRemaining.ToStringTicksToPeriod()).Resolve());
            }
            else
            {
                text.AppendInNewLine("OAGene_WorkPausedNoPower".Translate((60000 - powerCutTicks).ToStringTicksToPeriod().Named("TIME")).Colorize(ColorLibrary.RedReadable));
            }
        }
        return text.ToString();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref powerCutTicks, "powerCutTicks", 0);
        Scribe_Values.Look(ref allTicks, "allTicks", 0);
    }
}
