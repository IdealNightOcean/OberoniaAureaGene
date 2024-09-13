using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class GameCondition_ExtremeSnowstorm : GameCondition_ForceWeather
{
    private static readonly float SkyGlow = 0.25f;
    private static SkyColorSet SnowstormSkyColors = new(new Color(0.416f, 0.553f, 0.643f), new Color(0.92f, 0.92f, 0.92f), new Color(0.6f, 0.6f, 0.6f), 0.9f);

    public float tempOffset;
    protected bool causeColdSnap;
    protected bool musicPlayed;
    protected int musicDelay = 1200;
    public override int TransitionTicks => 5000;
    private readonly List<SkyOverlay> snowHardOverlay = [new WeatherOverlay_SnowExtreme()];

    public override void Init()
    {
        base.Init();
        tempOffset = def.temperatureOffset;
        weather = def.weatherDef;

        if (Rand.Chance(0.25f))
        {
            GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, this.Duration);
            gameConditionManager.RegisterCondition(gameCondition);
            Letter letter = LetterMaker.MakeLetter("OAGene_ExtremeSnowstormCauseColdSnapTitle".Translate(), "OAGene_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
            Find.LetterStack.ReceiveLetter(letter, playSound: false);
            Find.MusicManagerPlay.ForceFadeoutAndSilenceFor(21);
            causeColdSnap = true;
            musicDelay = 1200;
        }
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            map.weatherManager.TransitionTo(weather);
            map.GetOAGeneMapComp()?.Notify_Snow(Duration);
            TryBreakPowerPlantWind(map, Duration);
        }
    }
    public override void End()
    {
        base.End();
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            map.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowGentle);
        }
    }
    public override void GameConditionTick()
    {
        if (causeColdSnap && !musicPlayed)
        {
            musicDelay--;
            if (musicDelay <= 0)
            {
                Find.MusicManagerPlay.ForcePlaySong(OAGene_MiscDefOf.OAGene_SnowstormColdSnap, false);
                musicPlayed = true;
            }
        }
        base.GameConditionTick();
    }
    public override float TemperatureOffset()
    {
        return tempOffset;
    }
    public override SkyTarget? SkyTarget(Map map)
    {
        return new(SkyGlow, SnowstormSkyColors, 1f, 1f);
    }
    public override float SkyTargetLerpFactor(Map map)
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
    }
    public override List<SkyOverlay> SkyOverlays(Map map)
    {
        return snowHardOverlay;
    }
    public override void GameConditionDraw(Map map)
    {
        for (int i = 0; i < snowHardOverlay.Count; i++)
        {
            snowHardOverlay[i].DrawOverlay(map);
        }
    }
    protected static void TryBreakPowerPlantWind(Map map, int duration) //破坏风力发电机
    {
        BreakdownManager breakdownManager = map.GetComponent<BreakdownManager>();
        if (breakdownManager == null)
        {
            return;
        }
        List<CompBreakdownable> breakdownableComps = ReflectionUtility.GetFieldValue<List<CompBreakdownable>>(breakdownManager, "comps", null);
        if (breakdownableComps == null)
        {
            return;
        }
        CompPowerNormalPlantWind normalPlantWind;
        for (int i = 0; i < breakdownableComps.Count; i++)
        {
            normalPlantWind = breakdownableComps[i].parent.GetComp<CompPowerNormalPlantWind>();
            normalPlantWind?.Notify_ExtremeSnowstorm(duration);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
        Scribe_Values.Look(ref causeColdSnap, "causeColdSnap", defaultValue: false);
        Scribe_Values.Look(ref musicPlayed, "musicPlayed", defaultValue: false);
        Scribe_Values.Look(ref musicDelay, "musicDelay", 1200);
    }
}
