using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace OberoniaAureaGene.Ratkin;

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
    public float HeldPawnBodyAngle => Rotation.Opposite.AsAngle;
    public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;
    public override Vector3 PawnDrawOffset => IntVec3.West.RotatedBy(Rotation).ToVector3() / def.size.x;

    protected virtual string CommandInsertPersonStr => "OAGene_InsertPerson".Translate();
    protected virtual string CommandInsertPersonDescStr => "OAGene_InsertPersonDesc".Translate();

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        compPower = this.TryGetComp<CompPowerTrader>();
        placePos = def.hasInteractionCell ? InteractionCell : Position;
    }
    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        if (BeingTransportedOnGravship)
        {
            ClearEffects();
        }
        else
        {
            CancelWork();
        }

        base.DeSpawn(mode);
    }

    protected override void Tick()
    {
        base.Tick();
        innerContainer.DoTick();
        if (this.IsHashIntervalTick(250))
        {
            compPower.PowerOutput = (Working ? (0f - PowerComp.Props.PowerConsumption) : (0f - PowerComp.Props.idlePowerDraw));
        }
        if (Working)
        {
            if (ContainedPawn is null)
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
                    if (containedPawn is not null)
                    {
                        Messages.Message("GeneExtractorNoPowerEjectedMessage".Translate(containedPawn.Named("PAWN")), containedPawn, MessageTypeDefOf.NegativeEvent, historical: false);
                    }
                    CancelWork();
                    return;
                }
            }
        }
        else if (selectedPawn is not null && selectedPawn.Dead)
        {
            CancelWork();
        }
    }
    protected void TickEffects()
    {
        if (sustainerWorking is null || sustainerWorking.Ended)
        {
            sustainerWorking = SoundDefOf.GeneExtractor_Working.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
        }
        else
        {
            sustainerWorking.Maintain();
        }
        progressBar ??= EffecterDefOf.ProgressBarAlwaysVisible.Spawn();
        progressBar.EffectTick(new TargetInfo(Position + IntVec3.North.RotatedBy(Rotation), Map), TargetInfo.Invalid);
        MoteProgressBar mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
        if (mote is not null)
        {
            mote.progress = 1f - Mathf.Clamp01((float)ticksRemaining / allTicks);
            mote.offsetZ = ((Rotation == Rot4.North) ? 0.5f : (-0.5f));
        }
    }
    protected virtual void ClearEffects() //清理特效
    {
        sustainerWorking = null;
        progressBar?.Cleanup();
        progressBar = null;
    }

    public override AcceptanceReport CanAcceptPawn(Pawn pawn)
    {
        if (!pawn.IsColonist && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony && (!pawn.IsColonySubhuman || !pawn.IsGhoul))
        {
            return false;
        }
        if (selectedPawn is not null && selectedPawn != pawn)
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
        ClearEffects();
        powerCutTicks = 0;
        innerContainer.TryDropAll(def.hasInteractionCell ? InteractionCell : Position, Map, ThingPlaceMode.Near);
    }

    protected virtual void FinishWork()
    {
        innerContainer.TryDropAll(placePos, Map, ThingPlaceMode.Near);
        selectedPawn = null;
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
        if (fOpt is not null)
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
        else if (SelectedPawn == selPawn && !selPawn.IsPrisonerOfColony)
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
        if (Working)
        {
            Command_Action command_Action = new()
            {
                defaultLabel = "OAGene_CommandCancelWork".Translate(),
                defaultDesc = "OAGene_CommandCancelWorkDesc".Translate(),
                icon = OAFrame_IconUtility.CancelIcon,
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
        if (selectedPawn is not null)
        {
            Command_Action command_Action3 = new()
            {
                defaultLabel = "OAGene_CommandCancelLoad".Translate(),
                defaultDesc = "OAGene_CommandCancelLoadDesc".Translate(),
                icon = OAFrame_IconUtility.CancelIcon,
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
            icon = OAFrame_IconUtility.InsertPawnIcon,
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
        foreach (Pawn pawn in Map.mapPawns.AllPawnsSpawned)
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
        if (Working && ContainedPawn is not null)
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
        if (selectedPawn is not null && innerContainer.Count == 0)
        {
            text.AppendInNewLine("WaitingForPawn".Translate(selectedPawn.Named("PAWN")).Resolve());
        }
        else if (Working && ContainedPawn is not null)
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
