using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class CompProperties_SnowstormGeothermalGenerator : CompProperties
{
    public CompProperties_SnowstormGeothermalGenerator()
    {
        compClass = typeof(CompSnowstormGeothermalGenerator);
    }
}

public class CompSnowstormGeothermalGenerator : ThingComp
{
    public CompProperties_SnowstormGeothermalGenerator Props => props as CompProperties_SnowstormGeothermalGenerator;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        MapComponent_Snowstorm snowstormMapComp = parent.Map.SnowstormMapComp();
        if (snowstormMapComp is not null)
        {
            if (!snowstormMapComp.geothermalGenerators.Contains(parent))
            {
                snowstormMapComp.geothermalGenerators.Add(parent);
            }
        }
    }
    public override void PostDeSpawn(Map map)
    {
        MapComponent_Snowstorm snowstormMapComp = map.SnowstormMapComp();
        if (snowstormMapComp is not null)
        {
            if (snowstormMapComp.geothermalGenerators.Contains(parent))
            {
                snowstormMapComp.geothermalGenerators.Remove(parent);
            }
        }
        base.PostDeSpawn(map);
    }
}
