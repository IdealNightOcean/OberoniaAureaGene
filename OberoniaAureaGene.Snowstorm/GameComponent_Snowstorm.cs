using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_Snowstorm : GameComponent
{
    public static GameComponent_Snowstorm Instance { get; private set; }

    protected int snowstormCount;
    public int lastSnowstormStartTick = -1;
    public int lastSnowstormEndTick = -1;

    public int nextSnowstormMentalTick = -1;
    public int nextCultistConvertTick = -1;
    public bool CanGetSnowstormMentalNow => Find.TickManager.TicksGame > nextSnowstormMentalTick;
    public bool CanCultistConvertNow => Find.TickManager.TicksGame > nextCultistConvertTick;

    public bool starryNightTriggered;

    public bool SnowstormNow => snowstormCount > 0;

    protected int totalSnowstormCount;

    public GameComponent_Snowstorm(Game game) => Instance = this;
    public static void OpenDevWindow() => Find.WindowStack.Add(new DevWin_Snowstorm());

    public void DrawDevWindow(Listing_Standard listing_Rect)
    {
        listing_Rect.Label($"CurSnowstormCount: {snowstormCount}");
        listing_Rect.Label($"TotalSnowstormCount: {totalSnowstormCount}");
        listing_Rect.Label($"LastSnowstormStartTick: {lastSnowstormStartTick}");
        listing_Rect.Label($"LastSnowstormEndTick: {lastSnowstormEndTick}");
        listing_Rect.Gap(3f);
        listing_Rect.Label($"NextSnowstormMentalTick: {nextSnowstormMentalTick}");
        listing_Rect.Label($"CanGetSnowstormMentalNow: {CanGetSnowstormMentalNow}");
        listing_Rect.Label($"NextCultistConvertTick: {nextCultistConvertTick}");
        listing_Rect.Label($"CanCultistConvertNow: {CanCultistConvertNow}");
        listing_Rect.Gap(3f);
        listing_Rect.Label($"StarryNightTriggered: {starryNightTriggered}");
    }

    public override void LoadedGame()
    {
        try
        {
            foreach (QueuedIncident queuedInc in Find.Storyteller.incidentQueue)
            {
                if (queuedInc.FiringIncident.parms.target is null)
                {
                    queuedInc.FiringIncident.parms.target = Find.World;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error("Failed to fix the null parms.target for QueuedIncident.firingInc: " + ex);
        }
    }

    public void Notify_SnowstormStart()
    {
        snowstormCount++;
        lastSnowstormStartTick = Find.TickManager.TicksGame;
        totalSnowstormCount++;
    }

    public void Notify_SnowstormEnd()
    {
        snowstormCount = Mathf.Max(snowstormCount - 1, 0);
        lastSnowstormEndTick = Find.TickManager.TicksGame;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref snowstormCount, "snowstormCount", 0);
        Scribe_Values.Look(ref lastSnowstormStartTick, "lastSnowstormStartTick", -1);
        Scribe_Values.Look(ref lastSnowstormEndTick, "lastSnowstormEndTick", -1);

        Scribe_Values.Look(ref nextSnowstormMentalTick, "nextSnowstormMentalTick", -1);
        Scribe_Values.Look(ref nextCultistConvertTick, "nextCultistConvertTick", -1);

        Scribe_Values.Look(ref starryNightTriggered, "starryNightTriggered", defaultValue: false);
        Scribe_Values.Look(ref totalSnowstormCount, "totalSnowstormCount", 0);
    }
}
