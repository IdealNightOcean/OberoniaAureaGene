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
        listing_Rect.Label($"当前存在的极端风雪数: {snowstormCount}");
        listing_Rect.Label($"总共经历过的极端风雪数: {totalSnowstormCount}");
        listing_Rect.Label($"上次极端风雪开始Tick: {lastSnowstormStartTick}");
        listing_Rect.Label($"上次极端风雪结束Tick: {lastSnowstormEndTick}");
        listing_Rect.Gap(3f);
        listing_Rect.Label($"下一次极端风雪特殊精神状态判定Tick: {nextSnowstormMentalTick}");
        listing_Rect.Label($"当前是否可以判定极端风雪特殊精神状态: {CanGetSnowstormMentalNow}");
        listing_Rect.Label($"下一次风雪教徒可传教Tick: {nextCultistConvertTick}");
        listing_Rect.Label($"当前风雪教徒是否可传教: {CanCultistConvertNow}");
        listing_Rect.Gap(3f);
        listing_Rect.Label($"星月夜是否已触发: {starryNightTriggered}");
    }

    public override void LoadedGame()
    {
        TempNullParmsTargetFix();
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

    private void TempNullParmsTargetFix()
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
            Log.Error("[OAGene] Failed to fix the null parms.target for QueuedIncident.firingInc: " + ex);
        }
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
