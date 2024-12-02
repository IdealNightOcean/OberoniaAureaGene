using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class QuestPart_EndGame_SnowstoryMapGenerated : QuestPart
{
    public string inSignal;

    public override void Notify_QuestSignalReceived(Signal signal)
    {
        base.Notify_QuestSignalReceived(signal);
        if (signal.tag == inSignal)
        {
            Snowstorm_StoryUtility.StoryGameComp?.Notify_StoryInProgress();
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
    }
}
