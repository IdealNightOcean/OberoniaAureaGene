using RimWorld;
using UnityEngine;
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
            map.weatherManager.TransitionTo(Snowstrom_MiscDefOf.OAGene_IceSnowExtreme);
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
        WeatherDef weather = endSlience ? OAGene_RimWorldDefOf.SnowHard : Snowstrom_MiscDefOf.OAGene_SnowExtreme;
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];

            map.weatherManager.TransitionTo(weather);
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
            FleckDef fleckDef = Rand.Chance(0.85f) ? OAGene_MiscDefOf.OAGene_ColdGlow : OAGene_MiscDefOf.OAGene_BigColdGlow;
            Vector3 fleckLoc = new(c.x + FastEffectRandom.Next(1, 50) / 100f, 10.54054f, c.z + FastEffectRandom.Next(1, 50) / 100f);
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(fleckLoc, map, fleckDef, FastEffectRandom.Next(200, 300) / 100f);
            dataStatic.velocityAngle = FastEffectRandom.Next(0, 360);
            dataStatic.velocitySpeed = 0.08f;
            map.flecks.CreateFleck(dataStatic);
        }
    }
}
