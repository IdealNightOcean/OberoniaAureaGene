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

            EndGameConditions();
            Find.Storyteller.incidentQueue.Clear();

            Snowstorm_StoryUtility.StoryGameComp?.Notify_StoryInProgress();
        }
    }

    protected static void EndGameConditions()
    {
        GameConditionManager gameConditionManager = Find.World.gameConditionManager;
        gameConditionManager.GetActiveCondition<GameCondition_Icestorm>()?.EndSlience();
        gameConditionManager.GetActiveCondition<GameCondition_SnowstormPrecursor>()?.EndSlience();
        gameConditionManager.GetActiveCondition<GameCondition_ExtremeSnowstorm>()?.EndSlience();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
    }
}
