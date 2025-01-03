using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestPart_EndGame_CheckThreat : QuestPartActivable
{
    public const int CheckInterval = 5000;
    public string outSignalNoThreat;
    private int ticksRemaining = 5000;

    public MapParent hometown;
    public override void QuestPartTick()
    {
        base.QuestPartTick();

        ticksRemaining--;
        if (ticksRemaining <= 0)
        {
            Map map = hometown.Map;
            if (map != null)
            {
                int curHour = GenLocalDate.HourOfDay(map);
                if (curHour >= 9 && curHour <= 15)
                {
                    if (OAFrame_MapUtility.ThreatsCountOfPlayerOnMap(map) <= 0)
                    {
                        Complete();
                        Find.SignalManager.SendSignal(new Signal(outSignalNoThreat));
                    }
                }
            }
            ticksRemaining = CheckInterval;
        }
    }
    public override void Cleanup()
    {
        base.Cleanup();
        hometown = null;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref outSignalNoThreat, "outSignalNoThreat");
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_References.Look(ref hometown, "hometown");
    }
}
