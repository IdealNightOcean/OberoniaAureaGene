using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_IceRain : GameCondition_ForceWeatherWithTempChange
{
    protected static IntRange ColdGlowSpawnRange = new(10, 20);
    protected static IntRange ColdGlowIntervalRange = new(2400, 3000);

    protected int coldGlowSpawnTicks;
    protected bool coldGlowSpawn;
    public override void GameConditionTick()
    {
        coldGlowSpawnTicks--;
        if (coldGlowSpawnTicks < 0)
        {
            coldGlowSpawn = !coldGlowSpawn;
            coldGlowSpawnTicks = coldGlowSpawn ? ColdGlowSpawnRange.RandomInRange : ColdGlowIntervalRange.RandomInRange;
        }
    }
    public override void End()
    {
        base.End();
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            if (map.weatherManager.curWeather == Snowstorm_MiscDefOf.OAGene_IceRain)
            {
                map.weatherDecider.StartNextWeather();
            }
        }
    }

    public override void DoCellSteadyEffects(IntVec3 c, Map map)
    {
        if (!coldGlowSpawn)
        {
            return;
        }
        if (Random.value < 0.025f)
        {
            Vector3 fleckLoc = new(c.x + FastEffectRandom.Next(1, 50) / 100f, 10.54054f, c.z + FastEffectRandom.Next(1, 50) / 100f);
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(fleckLoc, map, OAGene_MiscDefOf.OAGene_ColdGlow, FastEffectRandom.Next(200, 300) / 100f);
            dataStatic.velocityAngle = FastEffectRandom.Next(0, 360);
            dataStatic.velocitySpeed = 0.08f;
            map.flecks.CreateFleck(dataStatic);
        }
    }
}