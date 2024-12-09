using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestPart_EndGame_EndEndGameSnowstorm : QuestPart
{
    public string inSignal;
    public int snowstormEndDelay;
    public override void Notify_QuestSignalReceived(Signal signal)
    {
        if (signal.tag == inSignal)
        {
            Find.MusicManagerPlay.ForceSilenceFor(180f);
            GameCondition_EndGame_ExtremeSnowstorm endGameSnowstorm = Find.World.gameConditionManager.GetActiveCondition(Snowstorm_MiscDefOf.OAGene_EndGame_ExtremeSnowstorm) as GameCondition_EndGame_ExtremeSnowstorm;
            endGameSnowstorm?.Notify_EndSnowstorm(snowstormEndDelay);
        }
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
        Scribe_Values.Look(ref snowstormEndDelay, "snowstormEndDelay");
    }
}