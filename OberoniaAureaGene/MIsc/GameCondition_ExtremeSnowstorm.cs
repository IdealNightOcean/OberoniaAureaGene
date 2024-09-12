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
    private static SkyColorSet SnowstormSkyColors = new(new Color(0.482f, 0.603f, 0.682f), new Color(0.92f, 0.92f, 0.92f), new Color(0.6f, 0.6f, 0.6f), 0.9f);

    public float tempOffset;
    public override int TransitionTicks => 5000;
    private readonly List<SkyOverlay> snowHardOverlay = [new WeatherOverlay_SnowExtreme()];

    public override void Init()
    {
        base.Init();
        tempOffset = def.temperatureOffset;
        weather = def.weatherDef;
        Find.MusicManagerPlay.ForcePlaySong(OberoniaAureaGeneDefOf.OAGene_ExtremeSnowstormStart, false);

        if (Rand.Chance(0.25f))
        {
            GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, this.Duration);
            gameConditionManager.RegisterCondition(gameCondition);
            Letter letter = LetterMaker.MakeLetter("OAGene_ExtremeSnowstormCauseColdSnapTitle".Translate(), "OAGene_ExtremeSnowstormCauseColdSnap".Translate(), OberoniaAureaGeneDefOf.OAGene_SnowstormColdSnap);
            Find.LetterStack.ReceiveLetter(letter);
        }
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            map.GetOAGeneMapComp()?.Notify_Snow(Duration);
            TryBreakPowerPlantWind(map);
        }
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
    protected static void TryBreakPowerPlantWind(Map map) //破坏风力发电机
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
        for (int i = 0; i < breakdownableComps.Count; i++)
        {
            CompBreakdownable breakComp = breakdownableComps[i];
            if (breakComp.parent.GetComp<CompPowerPlantWind>() != null)
            {
                breakComp.DoBreakdown();
            }
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
    }
}
