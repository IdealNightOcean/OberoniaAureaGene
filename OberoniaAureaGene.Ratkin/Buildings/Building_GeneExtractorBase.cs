using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace OberoniaAureaGene.Ratkin;

public abstract class Building_GeneExtractorBase : Building_Enterable, IThingHolderWithDrawnPawn, IThingHolder
{
    protected int ticksRemaining;
    protected int powerCutTicks;
    protected int ticksToExtract;
    protected List<GeneDef> targetGenes = [];

    [Unsaved]
    protected CompPowerTrader compPower;
    protected IntVec3 placePos;

    [Unsaved]
    protected Sustainer sustainerWorking;

    [Unsaved]
    protected Effecter progressBar;

    protected virtual int TicksToExtract => 30000;
    protected const int NoPowerEjectCumulativeTicks = 60000;

    protected Pawn ContainedPawn => innerContainer.FirstOrDefault() as Pawn;
    public bool PowerOn => compPower.PowerOn;

    protected virtual float ProgressBarOffsetZ => -0.8f;
    public override bool IsContentsSuspended => false;
    public float HeldPawnDrawPos_Y => DrawPos.y + 1f / 26f;
    public float HeldPawnBodyAngle => Rotation.AsAngle;
    public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;
    public override Vector3 PawnDrawOffset => Vector3.zero;

    protected virtual string CommandInsertPersonStr => "OAGene_InsertPerson".Translate();
    protected virtual string CommandInsertPersonDescStr => "OAGene_InsertPersonDesc".Translate();


    public override void PostPostMake()
    {
        if (!ModLister.CheckBiotech("gene extractor"))
        {
            Destroy();
        }
        else
        {
            base.PostPostMake();
        }
    }
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        compPower = this.TryGetComp<CompPowerTrader>();
        placePos = def.hasInteractionCell ? InteractionCell : Position;
    }
    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        sustainerWorking = null;
        if (progressBar is not null)
        {
            progressBar.Cleanup();
            progressBar = null;
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
        progressBar.EffectTick(this, TargetInfo.Invalid);
        MoteProgressBar mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
        if (mote is not null)
        {
            mote.progress = 1f - Mathf.Clamp01((float)ticksRemaining / ticksToExtract);
            mote.offsetZ = ProgressBarOffsetZ;
        }
    }

    protected void ClearEffects()
    {
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
        if (pawn.genes is null || !pawn.genes.GenesListForReading.Any((Gene x) => x.def.passOnDirectly))
        {
            return "PawnHasNoGenes".Translate(pawn.Named("PAWN"));
        }
        if (!pawn.genes.GenesListForReading.Any((Gene x) => x.def.biostatArc == 0))
        {
            return "PawnHasNoNonArchiteGenes".Translate(pawn.Named("PAWN"));
        }
        if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogerminationComa))
        {
            return "InXenogerminationComa".Translate();
        }
        return true;
    }

    public abstract void TryStartWork(Pawn pawn);
    public virtual void StartWork(Pawn pawn, List<GeneDef> geneDefs)
    {
        ticksToExtract = TicksToExtract;
        targetGenes.AddRange(geneDefs);
        SelectPawn(pawn);
    }
    public virtual void StartWork(Pawn pawn)
    {
        ticksToExtract = TicksToExtract;
        targetGenes.Clear();
        SelectPawn(pawn);
    }
    protected virtual void CancelWork()
    {
        startTick = -1;
        selectedPawn = null;
        sustainerWorking = null;
        ClearEffects();
        powerCutTicks = 0;
        targetGenes.Clear();
        innerContainer.TryDropAll(def.hasInteractionCell ? InteractionCell : Position, Map, ThingPlaceMode.Near);
    }

    protected virtual void FinishWork()
    {
        innerContainer.TryDropAll(placePos, Map, ThingPlaceMode.Near);
        targetGenes.Clear();
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
                ticksRemaining = ticksToExtract;
            }
            if (num)
            {
                Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);
            }
        }
    }
    protected override void SelectPawn(Pawn pawn)
    {
        if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmExtractXenogermWillKill".Translate(pawn.Named("PAWN")), delegate
            {
                base.SelectPawn(pawn);
            }));
        }
        else
        {
            base.SelectPawn(pawn);
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
                defaultLabel = "CommandCancelExtraction".Translate(),
                defaultDesc = "CommandCancelExtractionDesc".Translate(),
                icon = OAFrame_IconUtility.CancelIcon,
                action = CancelWork,
                activateSound = SoundDefOf.Designate_Cancel
            };
            yield return command_Action;
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_Action2 = new()
                {
                    defaultLabel = "DEV: Finish extraction",
                    action = FinishWork
                };
                yield return command_Action2;
            }
            yield break;
        }
        if (selectedPawn is not null)
        {
            Command_Action command_Action3 = new()
            {
                defaultLabel = "CommandCancelLoad".Translate(),
                defaultDesc = "CommandCancelLoadDesc".Translate(),
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
            defaultLabel = CommandInsertPersonStr,
            defaultDesc = CommandInsertPersonDescStr,
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
            if (pawn.genes is not null)
            {
                AcceptanceReport acceptanceReport = CanAcceptPawn(pawn);
                string text = pawn.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap;
                if (!acceptanceReport.Accepted)
                {
                    if (!acceptanceReport.Reason.NullOrEmpty())
                    {
                        list.Add(new FloatMenuOption(text + ": " + acceptanceReport.Reason, null, pawn, Color.white));
                    }
                }
                else
                {
                    Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
                    if (firstHediffOfDef is not null)
                    {
                        text = text + " (" + firstHediffOfDef.LabelBase + ", " + firstHediffOfDef.TryGetComp<HediffComp_Disappears>().ticksToDisappear.ToStringTicksToPeriod(allowSeconds: true, shortForm: true).Colorize(ColoredText.SubtleGrayColor) + ")";
                    }
                    list.Add(new FloatMenuOption(text, delegate
                    {
                        TryStartWork(pawn);
                    }, pawn, Color.white));
                }
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
        string text = base.GetInspectString();
        if (selectedPawn is not null && innerContainer.Count == 0)
        {
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            text += "WaitingForPawn".Translate(selectedPawn.Named("PAWN")).Resolve();
        }
        else if (Working && ContainedPawn is not null)
        {
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            text = text + "ExtractingXenogermFrom".Translate(ContainedPawn.Named("PAWN")).Resolve() + "\n";
            text = ((!PowerOn) ? (text + "ExtractionPausedNoPower".Translate((60000 - powerCutTicks).ToStringTicksToPeriod().Named("TIME")).Colorize(ColorLibrary.RedReadable)) : (text + "DurationLeft".Translate(ticksRemaining.ToStringTicksToPeriod()).Resolve()));
        }
        return text;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref powerCutTicks, "powerCutTicks", 0);
        Scribe_Values.Look(ref ticksToExtract, "ticksToExtract", 0);
        Scribe_Collections.Look(ref targetGenes, "targetGenes", LookMode.Def);
    }
}
