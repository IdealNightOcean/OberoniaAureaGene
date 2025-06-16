using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OberoniaAureaGene.Ratkin;

public abstract class Building_GeneDiscriminatorBase : Building
{
    public Genepack targetGenepack;
    protected GeneDef targetGeneDef;
    protected int startTick = -1;
    protected int ticksRemaining;
    protected int powerCutTicks;
    protected virtual int TicksToDiscriminat => 30000;

    protected CompPowerTrader compPower;
    protected CompGeneDiscriminat compGeneDiscriminat;
    public CompGeneDiscriminat CompGeneDiscriminat => compGeneDiscriminat;

    protected IntVec3 placePos;

    [Unsaved(false)]
    protected Sustainer sustainerWorking;

    [Unsaved(false)]
    protected Effecter progressBar;

    protected static int NoPowerEjectCumulativeTicks = 60000;
    protected virtual int AllTicks => 0;
    protected Genepack ContainedGenepack => CompGeneDiscriminat.ContainedGenepacks.FirstOrFallback();
    public bool PowerOn => compPower.PowerOn;
    public bool Working => startTick >= 0;
    public bool GenepackLoaded //基因组是否装载完毕
    {
        get
        {
            if (Working || targetGenepack is null)
            {
                return true;
            }
            if (ContainedGenepack is null)
            {
                return false;
            }
            return true;
        }
    }

    [Unsaved(false)]
    private readonly List<Genepack> tempGenepacks = [];
    public List<Thing> ConnectedFacilities => this.TryGetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading;
    public List<Genepack> GetGenepacks(bool includePowered, bool includeUnpowered)
    {
        tempGenepacks.Clear();
        List<Thing> connectedFacilities = ConnectedFacilities;
        if (connectedFacilities is not null)
        {
            foreach (Thing item in connectedFacilities)
            {
                CompGenepackContainer compGenepackContainer = item.TryGetComp<CompGenepackContainer>();
                if (compGenepackContainer is not null)
                {
                    bool flag = item.TryGetComp<CompPowerTrader>()?.PowerOn ?? true;
                    if ((includePowered && flag) || (includeUnpowered && !flag))
                    {
                        tempGenepacks.AddRange(compGenepackContainer.ContainedGenepacks);
                    }
                }
            }
        }
        return tempGenepacks;
    }

    public Thing GetTargetGeneBank()
    {
        if (targetGenepack is null)
        {
            return null;
        }
        List<Thing> connectedFacilities = ConnectedFacilities;
        if (connectedFacilities is not null)
        {
            foreach (Thing item in connectedFacilities)
            {
                CompGenepackContainer compGenepackContainer = item.TryGetComp<CompGenepackContainer>();
                if (compGenepackContainer is not null)
                {
                    if (compGenepackContainer.ContainedGenepacks.Contains(targetGenepack))
                    {
                        return item;
                    }
                }
            }
        }
        return null;
    }

    public CompGenepackContainer GetGeneBankHoldingPack(Genepack pack)
    {
        List<Thing> connectedFacilities = ConnectedFacilities;
        if (connectedFacilities is not null)
        {
            foreach (Thing item in connectedFacilities)
            {
                CompGenepackContainer compGenepackContainer = item.TryGetComp<CompGenepackContainer>();
                if (compGenepackContainer is null)
                {
                    continue;
                }
                foreach (Genepack containedGenepack in compGenepackContainer.ContainedGenepacks)
                {
                    if (containedGenepack == pack)
                    {
                        return compGenepackContainer;
                    }
                }
            }
        }
        return null;
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        compPower = this.TryGetComp<CompPowerTrader>();
        compGeneDiscriminat = this.TryGetComp<CompGeneDiscriminat>();
        placePos = base.def.hasInteractionCell ? InteractionCell : base.Position;
    }
    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        CancelWork();
        base.DeSpawn(mode);
    }

    protected override void Tick()
    {
        base.Tick();
        if (this.IsHashIntervalTick(250))
        {
            compPower.PowerOutput = (Working ? (0f - base.PowerComp.Props.PowerConsumption) : (0f - base.PowerComp.Props.idlePowerDraw));
        }
        if (Working)
        {
            if (PowerOn)
            {
                EffectsTick();
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
                    Genepack containedGenepack = ContainedGenepack;
                    if (containedGenepack is not null)
                    {
                        Messages.Message("OAGene_MessageGeneDiscriminatNoPowerEjected".Translate(), containedGenepack, MessageTypeDefOf.NegativeEvent, historical: false);
                    }
                    CancelWork();
                    return;
                }
            }
        }
    }
    protected void EffectsTick()
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
        progressBar.EffectTick(new TargetInfo(base.Position + IntVec3.North.RotatedBy(base.Rotation), base.Map), TargetInfo.Invalid);
        MoteProgressBar mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
        if (mote is not null)
        {
            mote.progress = 1f - Mathf.Clamp01((float)ticksRemaining / TicksToDiscriminat);
            mote.offsetZ = ((base.Rotation == Rot4.North) ? 0.5f : (-0.5f));
        }
    }
    protected virtual void ClearEffects() //清理特效
    {
        progressBar?.Cleanup();
        progressBar = null;
    }
    public virtual void Notify_GenepackLoaded()
    {
        startTick = Find.TickManager.TicksGame;
    }
    public virtual void TryStartWork(Genepack genepack, GeneDef geneDef)
    {
        compGeneDiscriminat.EjectContents(base.Map);
        ticksRemaining = TicksToDiscriminat;
        targetGenepack = genepack;
        targetGeneDef = geneDef;
        if (targetGenepack is not null)
        {
            if (targetGenepack.targetContainer is null)
            {
                targetGenepack.targetContainer = this;
            }
            else
            {
                CancelWork();
                Messages.Message("OAGene_GenepackUsedByOther".Translate(), targetGenepack, MessageTypeDefOf.RejectInput, historical: false);
                return;
            }

        }
        compGeneDiscriminat.leftToLoad.Add(targetGenepack);
        compGeneDiscriminat.autoLoad = false;
    }

    protected virtual void CancelWork()
    {
        startTick = -1;
        sustainerWorking = null;
        ClearEffects();
        powerCutTicks = 0;
        if (targetGenepack is not null && targetGenepack.targetContainer == this)
        {
            targetGenepack.targetContainer = null;
        }
        targetGenepack = null;
        targetGeneDef = null;
        compGeneDiscriminat.EjectContents(base.Map);
        compGeneDiscriminat.autoLoad = false;
    }

    protected virtual void FinishWork()
    {
        compGeneDiscriminat.EjectContents(base.Map);
        compGeneDiscriminat.autoLoad = false;
        if (targetGenepack is not null)
        {
            targetGenepack.targetContainer = null;
        }
        targetGenepack = null;
        targetGeneDef = null;
        sustainerWorking = null;
        ClearEffects();
        powerCutTicks = 0;
        startTick = -1;
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
        else
        {
            if (targetGenepack is null)
            {
                Command_Action command_Action3 = new()
                {
                    defaultLabel = "OAGene_CommandLoadDiscriminator".Translate(),
                    defaultDesc = "OAGene_CommandLoadDiscriminatorDesc".Translate(),
                    icon = OAFrame_IconUtility.RecombineIcon,
                    activateSound = SoundDefOf.Designate_Cancel,
                    action = delegate
                    {
                        Find.WindowStack.Add(new Dialog_CreateDiscriminatGene(this));
                    }
                };
                yield return command_Action3;

            }
            else
            {
                Command_Action command_Action4 = new()
                {
                    defaultLabel = "OAGene_CommandCancelLoad".Translate(),
                    defaultDesc = "OAGene_CommandCancelLoadDesc".Translate(),
                    icon = OAFrame_IconUtility.CancelIcon,
                    activateSound = SoundDefOf.Designate_Cancel,
                    action = CancelWork
                };
                yield return command_Action4;
            }
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
        if (targetGenepack is not null && !compGeneDiscriminat.Full)
        {
            text.AppendInNewLine("OAGene_WaitingForGenePack".Translate().Resolve());
        }
        else if (Working && ContainedGenepack is not null)
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
        Scribe_Values.Look(ref startTick, "startTick", 0);
        Scribe_References.Look(ref targetGenepack, "targetGenepack");
        Scribe_Defs.Look(ref targetGeneDef, "targetGeneDef");
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref powerCutTicks, "powerCutTicks", 0);
    }
}
