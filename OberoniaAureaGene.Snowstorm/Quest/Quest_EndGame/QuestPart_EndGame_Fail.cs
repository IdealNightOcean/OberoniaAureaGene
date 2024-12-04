using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class QuestPart_EndGame_Fail : QuestPart
{
    public string inSignal;

    public override void Notify_QuestSignalReceived(Signal signal)
    {
        if (signal.tag == inSignal)
        {
            Snowstorm_StoryUtility.StoryGameComp?.Notify_StroyFail();
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
    }
}
