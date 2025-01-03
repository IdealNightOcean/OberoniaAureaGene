using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestPart_EndGame_SignalProtagonistFail : QuestPartActivable
{
    public Pawn protagonist;
    public MapParent hometown;
    public string protagonistLeftMapSignal;
    public string protagonistDeadSignal;
    public string outSignalFail;
    protected override void ProcessQuestSignal(Signal signal)
    {
        if (signal.tag == protagonistLeftMapSignal)
        {
            if (protagonist.Map == null || protagonist.Map != hometown.Map)
            {
                Complete();
                Find.SignalManager.SendSignal(new Signal(outSignalFail));
            }
        }
        if (signal.tag == protagonistDeadSignal)
        {
            if (protagonist.MapHeld == hometown.Map)
            {
                Complete();
                Find.SignalManager.SendSignal(new Signal(outSignalFail));
            }
        }
    }
    public override void Cleanup()
    {
        base.Cleanup();
        protagonist = null;
        hometown = null;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref protagonist, "protagonist");
        Scribe_References.Look(ref hometown, "hometown");

        Scribe_Values.Look(ref protagonistLeftMapSignal, "protagonistLeftMapSignal");
        Scribe_Values.Look(ref protagonistDeadSignal, "protagonistDeadSignal");
        Scribe_Values.Look(ref outSignalFail, "outSignalFail");
    }
}
