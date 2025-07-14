using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class Gene_BillInspiration : Gene
{
    private int ticksRemaining = 60000;
    private int remainingCDays;
    public bool Cooling => remainingCDays > 0;

    public override void PostAdd()
    {
        base.PostAdd();
        remainingCDays = 0;
        ticksRemaining = 60000;
    }
    public override void Tick()
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            remainingCDays--;
            ticksRemaining = 60000;
        }
    }

    public void TryGetBillInspiration()
    {
        if (Cooling)
        {
            return;
        }
        if (!InspirationDefOf.Inspired_Creativity.Worker.InspirationCanOccur(pawn))
        {
            return;
        }
        if (pawn.mindState.inspirationHandler.Inspired)
        {
            return;
        }
        float chance = 0.01f;
        Need_Mood pawnMood = pawn.needs.mood;
        if (pawnMood is not null)
        {
            float validPercentage = (pawnMood.CurInstantLevelPercentage - 0.5f) * 0.1f;
            chance += validPercentage > 0f ? validPercentage : 0f;
        }
        if (Rand.Chance(chance))
        {
            pawn.mindState.inspirationHandler.TryStartInspiration(InspirationDefOf.Inspired_Creativity, "OAGene_LetterBillInspiration".Translate(pawn.Named("PAWN")));
            Notify_SuccessfullyInspired();
        }
    }

    protected void Notify_SuccessfullyInspired()
    {
        remainingCDays = 20;
        ticksRemaining = 60000;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref remainingCDays, "remainingCDays", 0);
    }
}
