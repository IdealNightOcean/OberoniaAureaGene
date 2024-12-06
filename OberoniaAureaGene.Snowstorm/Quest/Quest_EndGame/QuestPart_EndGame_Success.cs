﻿using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class QuestPart_EndGame_Success : QuestPart
{
    public string inSignal;
    public Pawn protagonist;
    public MapParent hometown;

    public override void Notify_QuestSignalReceived(Signal signal)
    {
        if (signal.tag == inSignal)
        {
            Map map = hometown.Map;
            End_EndGameSnowstorm(map);
            bool onlyProtagonist = (map?.mapPawns.FreeColonistsSpawnedCount ?? 0) == 1;
            Snowstorm_StoryUtility.StoryGameComp?.Notify_StroySuccess(onlyProtagonist);
        }
    }
    protected static void End_EndGameSnowstorm(Map map)
    {
        GameCondition_EndGame_ExtremeSnowstorm endGameSnowstorm = map?.gameConditionManager.GetActiveCondition(Snowstorm_MiscDefOf.OAGene_EndGame_ExtremeSnowstorm) as GameCondition_EndGame_ExtremeSnowstorm;
        endGameSnowstorm ??= Find.World.gameConditionManager.GetActiveCondition(Snowstorm_MiscDefOf.OAGene_EndGame_ExtremeSnowstorm) as GameCondition_EndGame_ExtremeSnowstorm;
        endGameSnowstorm?.Notify_EndGame();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");

        Scribe_References.Look(ref protagonist, "protagonist");
        Scribe_References.Look(ref hometown, "hometown");
    }
}
