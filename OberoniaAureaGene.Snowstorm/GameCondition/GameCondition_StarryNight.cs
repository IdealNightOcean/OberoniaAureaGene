using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_StarryNight : GameCondition
{
    public const float MaxSunGlow = 0.5f;
    private const float Glow = 0.25f;
    private const float SkyColorStrength = 0.075f;
    private const float OverlayColorStrength = 0.025f;
    private const float BaseBrightness = 0.73f;
    private const int TransitionDurationTicks_NotPermanent = 280;
    private const int TransitionDurationTicks_Permanent = 3750;
    private static readonly Color[] Colors =
    [
        new Color(0f, 1f, 0f),
        new Color(0.3f, 1f, 0f),
        new Color(0f, 1f, 0.7f),
        new Color(0.3f, 1f, 0.7f),
        new Color(0f, 0.5f, 1f),
        new Color(0f, 0f, 1f),
        new Color(0.87f, 0f, 1f),
        new Color(0.75f, 0f, 1f)
    ];

    public float tempOffset;
    public WeatherDef weather;

    private int curColorIndex = -1;
    private int prevColorIndex = -1;
    private float curColorTransition;

    protected static IntRange StarryGlowSpawnRange = new(40, 80);
    protected static IntRange StarryGlowIntervalRange = new(2400, 300);

    protected int starryGlowSpawnTicks;
    protected bool starryGlowSpawn;

    public Color CurrentColor => Color.Lerp(Colors[prevColorIndex], Colors[curColorIndex], curColorTransition);

    private int TransitionDurationTicks
    {
        get
        {
            if (Permanent)
            {
                return TransitionDurationTicks_Permanent;
            }
            return TransitionDurationTicks_NotPermanent;
        }
    }

    private bool BrightInAllMaps
    {
        get
        {
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                if (GenCelestial.CurCelestialSunGlow(maps[i]) <= MaxSunGlow)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public override int TransitionTicks => 200;

    public override void Init()
    {
        base.Init();
        curColorIndex = Rand.Range(0, Colors.Length);
        prevColorIndex = curColorIndex;
        curColorTransition = 1f;

        weather = def.weatherDef;
        tempOffset = def.temperatureOffset;

        PostInit();
    }
    protected void PostInit()
    {
        Find.MusicManagerPlay.ForceTriggerTransition(Snowstrom_MiscDefOf.OAGene_Transition_StarryNight);
        foreach (Map map in AffectedMaps)
        {
            map.weatherManager.TransitionTo(weather);
        }
        if (Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist))
        {
            if (OAFrame_PawnUtility.PawnSleepNow(protagonist))
            {
                protagonist.needs.mood?.thoughts.memories.TryGainMemory(Snowstrom_ThoughtDefOf.OAGene_Thought_StarryNightP);
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
    public override SkyTarget? SkyTarget(Map map)
    {
        Color currentColor = CurrentColor;
        return new SkyTarget(colorSet: new SkyColorSet(Color.Lerp(Color.white, currentColor, SkyColorStrength) * Brightness(map), new Color(0.92f, 0.92f, 0.92f), Color.Lerp(Color.white, currentColor, OverlayColorStrength) * Brightness(map), 1f), glow: Mathf.Max(GenCelestial.CurCelestialSunGlow(map), Glow), lightsourceShineSize: 1f, lightsourceShineIntensity: 1f);
    }
    private float Brightness(Map map)
    {
        return Mathf.Max(BaseBrightness, GenCelestial.CurCelestialSunGlow(map));
    }

    public override void GameConditionTick()
    {
        curColorTransition += 1f / TransitionDurationTicks;
        if (curColorTransition >= 1f)
        {
            prevColorIndex = curColorIndex;
            curColorIndex = GetNewColorIndex();
            curColorTransition = 0f;
        }

        StarryGlowTick();
    }

    public void StarryGlowTick()
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
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(fleckLoc, map, Snowstrom_MiscDefOf.OAGene_StarryGlow, FastEffectRandom.Next(200, 300) / 100f);
            dataStatic.velocityAngle = FastEffectRandom.Next(0, 360);
            dataStatic.velocitySpeed = 0.08f;
            map.flecks.CreateFleck(dataStatic);
        }
    }
    private int GetNewColorIndex()
    {
        return (from x in Enumerable.Range(0, Colors.Length)
                where x != curColorIndex
                select x).RandomElement();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref curColorIndex, "curColorIndex", 0);
        Scribe_Values.Look(ref prevColorIndex, "prevColorIndex", 0);
        Scribe_Values.Look(ref curColorTransition, "curColorTransition", 0f);

        Scribe_Defs.Look(ref weather, "weather");
        Scribe_Values.Look(ref tempOffset, "tempOffset", 0f);
    }
}

