using Verse;

namespace OberoniaAureaGene;

public class Gene_BillInspiration : Gene
{
    private int ticksRemaining = 60000;
    private int remainingCDays;
    public bool Cooling => remainingCDays > 0;

    public override void Tick()
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            remainingCDays--;
        }
    }
    public void Notify_SuccessfullyInspired()
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
