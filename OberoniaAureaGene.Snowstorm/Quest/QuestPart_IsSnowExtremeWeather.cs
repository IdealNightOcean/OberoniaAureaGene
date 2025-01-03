using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestPart_IsSnowExtremeWeather : QuestPartActivable
{
    public int checkInterval = 2500;

    public string outSignalSnowstorm;
    public string outSignalNotSnowstorm;

    public Map map;

    public bool snowstormOutSignal;
    public bool notSnowstormOutSignal;

    private int ticksRemaining = 2500;

    public override void QuestPartTick()
    {
        base.QuestPartTick();

        ticksRemaining--;
        if (ticksRemaining <= 0)
        {
            if (SnowstormUtility.IsSnowExtremeWeather(map))
            {
                if (snowstormOutSignal)
                {
                    Complete();
                    Find.SignalManager.SendSignal(new Signal(outSignalSnowstorm));
                }
            }
            else if (notSnowstormOutSignal)
            {
                Complete();
                Find.SignalManager.SendSignal(new Signal(outSignalNotSnowstorm));
            }
            ticksRemaining = checkInterval;
        }
    }
    public override void Cleanup()
    {
        base.Cleanup();
        map = null;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref outSignalSnowstorm, "outSignalSnowstorm");
        Scribe_Values.Look(ref outSignalNotSnowstorm, "outSignalNotSnowstorm");
        Scribe_Values.Look(ref snowstormOutSignal, "snowstormOutSignal", defaultValue: false);
        Scribe_Values.Look(ref notSnowstormOutSignal, "notSnowstormOutSignal", defaultValue: false);
        Scribe_Values.Look(ref checkInterval, "checkInterval", 2500);
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 2500);
        Scribe_References.Look(ref map, "map");
    }
}

