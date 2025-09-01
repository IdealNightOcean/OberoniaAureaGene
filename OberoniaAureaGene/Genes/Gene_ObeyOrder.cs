using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class Gene_ObeyOrderBase : Gene
{
    protected int lastDraftTick;
    [Unsaved] protected int tickToNextCheck = 500;

    protected virtual int NonDraftStartDay => 2;

    [Unsaved] protected ObeyOrderGeneExtension hediffRecord;
    public ObeyOrderGeneExtension HediffRecord => hediffRecord ??= def.GetModExtension<ObeyOrderGeneExtension>();

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref lastDraftTick, "lastDraftTick");
    }

    public override void PostAdd()
    {
        base.PostAdd();
        if (HediffRecord.hediffToWholeBody is not null)
        {
            pawn.AdjustOrAddHediff(HediffRecord.hediffToWholeBody);
        }
    }
    public override void PostRemove()
    {
        base.PostRemove();
        if (HediffRecord.hediffToWholeBody is not null)
        {
            pawn.RemoveFirstHediffOfDef(HediffRecord.hediffToWholeBody);
        }
    }

    public override void TickInterval(int delta)
    {
        base.TickInterval(delta);
        if ((tickToNextCheck -= delta) <= 0)
        {
            tickToNextCheck = 500;
            HediffCheck();
        }
    }

    protected virtual void HediffCheck()
    {
        if (pawn.timetable?.CurrentAssignment == TimeAssignmentDefOf.Work)
        {
            pawn.AdjustOrAddHediff(HediffRecord.hediffWorking, overrideDisappearTicks: 600);
        }
        if (pawn.Drafted)
        {
            lastDraftTick = Find.TickManager.TicksGame;
        }
        else
        {
            int nonDraftDay = (int)((Find.TickManager.TicksGame - lastDraftTick) / 60000f) - NonDraftStartDay;
            if (nonDraftDay >= 0)
            {
                pawn.AdjustOrAddHediff(HediffRecord.hediffNonDraft, severity: nonDraftDay, overrideDisappearTicks: 600);
            }

        }
    }
}


public class Gene_ObeyOrder : Gene_ObeyOrderBase
{
    protected override int NonDraftStartDay => 3;

    protected override void HediffCheck()
    {
        base.HediffCheck();
        if (pawn.Drafted || (pawn.CurJob?.playerForced ?? false))
        {
            pawn.AdjustOrAddHediff(OAGene_HediffDefOf.OAGene_ObeyOrderForceJob, overrideDisappearTicks: 600);
        }
    }
}