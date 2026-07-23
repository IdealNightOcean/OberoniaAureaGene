using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_SnowstormFog : GameCondition
{
    protected static IntRange ColdGlowSpawnRange = new(30, 60);
    protected static IntRange ColdGlowIntervalRange = new(1200, 1500);

    protected int coldGlowSpawnTicks;
    protected bool coldGlowSpawn;

    public override void Init()
    {
        base.Init();
        SingleMap?.SnowstormMapComp()?.Notify_SnowstormFog(state: true);
    }
    public override void End()
    {
        base.End();
        SingleMap?.SnowstormMapComp()?.Notify_SnowstormFog(state: false);
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
        if (coldGlowSpawn)
        {
            OAGeneUtility.SpawnColdGlowFleck(map: map, position: c, spawnChance: 0.025f, bigGlowSpawnChance: 0.05f);
        }
    }
}
