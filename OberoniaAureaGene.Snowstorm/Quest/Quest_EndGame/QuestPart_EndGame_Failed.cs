using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class QuestPart_EndGame_Failed : QuestPart
{
    public string inSignal;

    public override void Notify_QuestSignalReceived(Signal signal)
    {
        if (signal.tag == inSignal)
        {
            Snowstorm_StoryUtility.StoryGameComp?.Notify_StroyFailed();
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
    }
}
