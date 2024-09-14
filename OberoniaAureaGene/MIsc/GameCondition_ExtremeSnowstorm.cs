using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class GameCondition_ExtremeSnowstorm : GameCondition_ForceWeather
{
    protected static readonly float SkyGlow = 0.25f;
    protected static SkyColorSet SnowstormSkyColors = new(new Color(0.416f, 0.553f, 0.643f), new Color(0.92f, 0.92f, 0.92f), new Color(0.6f, 0.6f, 0.6f), 0.9f);

    protected static IntRange ColdGlowSpawnRange = new(30, 60);
    protected static IntRange ColdGlowIntervalRange = new(1200, 1500);

    public float tempOffset;

    public bool causeColdSnap;
    protected int coldGlowSpawnTicks;
    protected bool coldGlowSpawn;
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
            Find.MusicManagerPlay.ForceTriggerTransition(OAGene_MiscDefOf.OAGene_Transition_ClairDeLune);
            causeColdSnap = true;
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
    public override void GameConditionTick()
    {
        coldGlowSpawnTicks--;
        if (coldGlowSpawnTicks < 0)
        {
            coldGlowSpawn = !coldGlowSpawn;
            coldGlowSpawnTicks = coldGlowSpawn ? ColdGlowSpawnRange.RandomInRange : ColdGlowIntervalRange.RandomInRange;
        }
    }
    public override void DoCellSteadyEffects(IntVec3 c, Map map)
    {
        if (!coldGlowSpawn)
        {
            return;
        }
        if (Rand.Chance(0.02f))
        {
            Vector3 fleckLoc = new(c.x + FastEffectRandom.Next(1, 50) / 100f, 10.54054f, c.z + FastEffectRandom.Next(1, 50) / 100f);
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(fleckLoc, map, OAGene_MiscDefOf.OAGene_ColdGlow, FastEffectRandom.Next(200, 300) / 100f);
            //dataStatic.rotationRate = FastEffectRandom.Next(-300, 300) / 100f;
            dataStatic.velocityAngle = FastEffectRandom.Next(0, 360);
            dataStatic.velocitySpeed = 0.04f;
            map.flecks.CreateFleck(dataStatic);
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
    }
}
