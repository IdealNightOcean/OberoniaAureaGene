using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestPart_EndGame_CheckCampfire : QuestPartActivable
{
    public const int CheckInterval = 15000;
    public string outSignalNoValidCampfire;
    private int ticksRemaining = 15000;
    private bool secondViolate;

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
                bool flag = map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.Campfire).Where(t => (t.TryGetComp<CompRefuelable>()?.HasFuel ?? false)).Any();
                if (flag)
                {
                    secondViolate = false;
                }
                else if (secondViolate)
                {
                    Complete();
                    Find.SignalManager.SendSignal(new Signal(outSignalNoValidCampfire));
                }
                else
                {
                    Find.LetterStack.ReceiveLetter("OAGene_LetterLabel_NoValidCampfire".Translate(), "OAGene_Letter_NoValidCampfire".Translate(), LetterDefOf.NegativeEvent);
                    secondViolate = true;
                }
            }
            ticksRemaining = CheckInterval;
        }
    }
    public void SetFirstCheckDelay(int delay)
    {
        ticksRemaining = delay;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref outSignalNoValidCampfire, "outSignalNoValidCampfire");
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref secondViolate, "secondViolate", defaultValue: false);
        Scribe_References.Look(ref hometown, "hometown");
    }
}
