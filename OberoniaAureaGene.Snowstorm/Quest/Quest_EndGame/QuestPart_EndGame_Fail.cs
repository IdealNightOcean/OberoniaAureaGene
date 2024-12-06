﻿using RimWorld;
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
            End_EndGameSnowstorm(hometown.Map);
            Snowstorm_StoryUtility.StoryGameComp.Notify_StroyFail();
        }
    }
    protected static void End_EndGameSnowstorm(Map map)
    {
        GameCondition_EndGame_ExtremeSnowstorm endGameSnowstorm = map?.gameConditionManager.GetActiveCondition(Snowstorm_MiscDefOf.OAGene_EndGame_ExtremeSnowstorm) as GameCondition_EndGame_ExtremeSnowstorm;
        endGameSnowstorm ??= Find.World.gameConditionManager.GetActiveCondition(Snowstorm_MiscDefOf.OAGene_EndGame_ExtremeSnowstorm) as GameCondition_EndGame_ExtremeSnowstorm;
        endGameSnowstorm?.Notify_QuestFailed();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
        Scribe_References.Look(ref hometown, "hometown");
    }
}
