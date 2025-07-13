using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

public class GoodwillSituationWorker_HasHegemonicFlag : GoodwillSituationWorker
{
    private static SimpleValueCache<bool> ValueCache = new(cacheInterval: 30000, checker: IsPlayerHasHegemonicFlag);
    public override int GetNaturalGoodwillOffset(Faction other)
    {
        if (!ModsConfig.IdeologyActive || !other.IsPlayerSafe())
        {
            return 0;
        }

        return ValueCache.GetCachedResult() ? def.naturalGoodwillOffset : 0;
    }

    private static bool IsPlayerHasHegemonicFlag()
    {
        IEnumerable<Map> playerHomes = Find.Maps.Where(m => m.IsPlayerHome);
        foreach (Map map in playerHomes)
        {
            if (OAFrame_MapUtility.GetSpecialBuildingManager(map)?.HasBuilding(OAGene_MiscDefOf.OAGene_HegemonicFlag) ?? false)
            {
                return true;
            }
        }

        return false;
    }

    public static void ResetStaticCache()
    {
        ValueCache.Reset();
    }
}