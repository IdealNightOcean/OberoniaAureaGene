using Verse;

namespace OberoniaAureaGene.Snowstorm;


[StaticConstructorOnStartup]
public static class Snowstorm_MiscUtility
{
    public static GameComponent_Snowstorm SnowstormGameComp => Current.Game.GetComponent<GameComponent_Snowstorm>();

    public static MapComponent_Snowstorm SnowstormMapComp(this Map map)
    {
        return map?.GetComponent<MapComponent_Snowstorm>();
    }
}