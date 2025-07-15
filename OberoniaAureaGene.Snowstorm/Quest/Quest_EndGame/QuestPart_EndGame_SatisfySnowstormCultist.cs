using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class QuestPart_EndGame_SatisfySnowstormCultist : QuestPart
{
    public string inSignal;
    public override void Notify_QuestSignalReceived(Signal signal)
    {
        if (signal.tag == inSignal)
        {
            GameComponent_SnowstormStory.Instance.satisfySnowstormCultist = true;
        }
    }
}
