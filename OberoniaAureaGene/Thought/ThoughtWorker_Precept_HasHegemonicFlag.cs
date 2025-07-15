using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

// 需要清理静态缓存
public class ThoughtWorker_Precept_HasHegemonicFlag : ThoughtWorker_Precept
{
    private static SimpleMapCahce<bool> MapCache = new(cacheInterval: 30000, defaultValue: false, onlyPlayerHome: true, HasHegemonicFlag);
    protected override ThoughtState ShouldHaveThought(Pawn p)
    {
        if (!ModsConfig.IdeologyActive || !p.Faction.IsPlayerSafe())
        {
            return ThoughtState.Inactive;
        }
        if (MapCache.GetCachedResult(p.Map))
        {
            return ThoughtState.ActiveDefault;
        }
        return ThoughtState.Inactive;
    }

    private static bool HasHegemonicFlag(Map map)
    {
        return OAFrame_MapUtility.GetSpecialBuildingManager(map)?.HasBuilding(OAGene_MiscDefOf.OAGene_HegemonicFlag) ?? false;
    }

    public static void ClearStaticCache()
    {
        MapCache.Reset();
    }
}
