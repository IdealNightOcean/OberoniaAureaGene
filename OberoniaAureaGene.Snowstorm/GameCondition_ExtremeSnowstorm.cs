using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class GameCondition_ExtremeSnowstorm : GameCondition_ExtremeSnowstormBase
{
    public override void Init()
    {
        base.Init();
        SnowstormUtility.InitExtremeSnowstormWorld(gameConditionManager.ownerMap, Duration);
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.InitExtremeSnowstormLocal(map, Duration);
        }
    }
    protected override void PreEnd()
    {
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.EndExtremeSnowstormLocal(map);
        }
    }
}