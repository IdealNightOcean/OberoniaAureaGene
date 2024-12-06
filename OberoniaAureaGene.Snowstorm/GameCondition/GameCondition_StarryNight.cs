using OberoniaAurea_Frame;
using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_StarryNight : GameCondition
{
    public const float MaxSunGlow = 0.5f;

    public float tempOffset;
    public WeatherDef weather;

    protected static IntRange StarryGlowSpawnRange = new(30, 60);
    protected static IntRange StarryGlowIntervalRange = new(2400, 3000);

    protected int starryGlowSpawnTicks;
    protected bool starryGlowSpawn;

    public override int TransitionTicks => 200;

    public override void Init()
    {
        base.Init();

        weather = def.weatherDef;
        tempOffset = def.temperatureOffset;
        GameComponent_Snowstorm snowstormComp = Snowstorm_MiscUtility.SnowstormGameComp;
        if (snowstormComp != null)
        {
            snowstormComp.starryNightTriggered = true;
        }

        PostInit();
    }
    protected void PostInit()
    {
        Find.MusicManagerPlay.ForceTriggerTransition(Snowstorm_MiscDefOf.OAGene_Transition_StarryNight);
        foreach (Map map in AffectedMaps)
        {
            map.weatherManager.TransitionTo(weather);
        }
        if (Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist))
        {
            if (OAFrame_PawnUtility.PawnSleepNow(protagonist))
            {
                protagonist.needs.mood?.thoughts.memories.TryGainMemory(Snowstorm_ThoughtDefOf.OAGene_Thought_StarryNightP);
            }
            InspirationDef inspirationDef = protagonist.mindState.inspirationHandler.GetRandomAvailableInspirationDef();
            if (inspirationDef != null)
            {
                protagonist.mindState.inspirationHandler.TryStartInspiration(inspirationDef);
            }
        }
    }
    public override float TemperatureOffset()
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks, tempOffset);
    }
    public override WeatherDef ForcedWeather()
    {
        return weather;
    }
    public override float SkyGazeChanceFactor(Map map)
    {
        return 8f;
    }
    public override float SkyGazeJoyGainFactor(Map map)
    {
        return 5f;
    }
    public override float SkyTargetLerpFactor(Map map)
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
    }

    public override void GameConditionTick()
    {
        starryGlowSpawnTicks--;
        if (starryGlowSpawnTicks < 0)
        {
            starryGlowSpawn = !starryGlowSpawn;
            starryGlowSpawnTicks = starryGlowSpawn ? StarryGlowSpawnRange.RandomInRange : StarryGlowIntervalRange.RandomInRange;
        }
    }

    public override void DoCellSteadyEffects(IntVec3 c, Map map)
    {
        if (!starryGlowSpawn)
        {
            return;
        }
        if (Random.value < 0.025f)
        {
            if (GenCelestial.CurCelestialSunGlow(map) > MaxSunGlow)
            {
                return;
            }
            Vector3 fleckLoc = new(c.x + FastEffectRandom.Next(1, 50) / 100f, 10.54054f, c.z + FastEffectRandom.Next(1, 50) / 100f);
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(fleckLoc, map, Snowstorm_MiscDefOf.OAGene_StarryGlow, FastEffectRandom.Next(200, 300) / 100f);
            dataStatic.velocityAngle = FastEffectRandom.Next(0, 360);
            dataStatic.velocitySpeed = 0.08f;
            map.flecks.CreateFleck(dataStatic);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Defs.Look(ref weather, "weather");
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
    }
}

