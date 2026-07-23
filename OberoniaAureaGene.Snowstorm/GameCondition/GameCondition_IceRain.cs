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
        if (coldGlowSpawn)
        {
            OAGeneUtility.SpawnColdGlowFleck(map: map, position: c, spawnChance: 0.025f, bigGlowSpawnChance: 0f);
        }
    }
}