using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class GameCondition_ExtremeSnowstorm : GameCondition_ExtremeSnowstormBase
{
    protected Map mainMap;
    public Map MainMap
    {
        get
        {
            if (mainMap == null)
            {
                if (gameConditionManager.ownerMap != null)
                {
                    mainMap = gameConditionManager.ownerMap;
                }
                else
                {
                    mainMap = AffectedMaps.Where(m => m.IsPlayerHome).RandomElementWithFallback(null);
                }
            }
            return mainMap;
        }
    }
    protected override void PostInit()
    {
        Current.Game.GetComponent<GameComponent_Snowstorm>()?.Notify_SnowstormStart();
        TryAddColdSnap();
        SnowstormUtility.InitExtremeSnowstorm_MainMap(MainMap, Duration);
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.InitExtremeSnowstorm_AllMaps(map, Duration);
        }
    }
    protected override void PreEnd()
    {
        Current.Game.GetComponent<GameComponent_Snowstorm>()?.Notify_SnowstormEnd();
        SnowstormUtility.EndExtremeSnowstorm_MainMap(MainMap);
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.EndExtremeSnowstorm_AllMaps(map);
        }
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref mainMap, "mainMap");
    }
}