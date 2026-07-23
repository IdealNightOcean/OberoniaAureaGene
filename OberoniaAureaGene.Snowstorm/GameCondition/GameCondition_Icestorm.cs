using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_Icestorm : GameCondition_ExtremeSnowstormBase
{
    [Unsaved]
    protected bool endSlience;
    public override int TransitionTicks => 2500;
    protected override void PostInit()
    {
        causeColdSnap = false;
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            map.weatherManager.TransitionTo(Snowstorm_MiscDefOf.OAGene_IceSnowExtreme);
        }
    }
    public void EndSlience()
    {
        endSlience = true;
        suppressEndMessage = true;
        End();
    }
    protected override void PreEnd()
    {
        WeatherDef weather = endSlience ? OAGene_RimWorldDefOf.SnowHard : Snowstorm_MiscDefOf.OAGene_SnowExtreme;
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            map.weatherManager.TransitionTo(weather);
        }
    }
    public override void DoCellSteadyEffects(IntVec3 c, Map map)
    {
        if (coldGlowSpawn)
        {
            OAGeneUtility.SpawnColdGlowFleck(map: map, position: c, spawnChance: 0.025f, bigGlowSpawnChance: 0.15f);
        }
    }
}
