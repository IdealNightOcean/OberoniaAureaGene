using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class QuestPart_EndGame_Fail : QuestPart
{
    public string inSignal;

    public MapParent hometown;

    public override void Notify_QuestSignalReceived(Signal signal)
    {
        if (signal.tag == inSignal)
        {
            End_EndGameSnowstrom(hometown.Map);
            Snowstorm_StoryUtility.StoryGameComp.Notify_StroyFail();
        }
    }
    protected static void End_EndGameSnowstrom(Map map)
    {
        GameCondition_EndGame_ExtremeSnowstorm endGameSnowstrom = map?.gameConditionManager.GetActiveCondition(Snowstrom_MiscDefOf.OAGene_EndGame_ExtremeSnowstorm) as GameCondition_EndGame_ExtremeSnowstorm;
        endGameSnowstrom ??= Find.World.gameConditionManager.GetActiveCondition(Snowstrom_MiscDefOf.OAGene_EndGame_ExtremeSnowstorm) as GameCondition_EndGame_ExtremeSnowstorm;
        endGameSnowstrom?.Notify_QuestFailed();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
        Scribe_References.Look(ref hometown, "hometown");
    }
}
