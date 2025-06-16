using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

public class GoodwillSituationWorker_HasHegemonicFlag : GoodwillSituationWorker
{
    public override int GetNaturalGoodwillOffset(Faction other)
    {
        if (ModsConfig.IdeologyActive)
        {
            IEnumerable<Map> playerHomes = Find.Maps.Where(m => m.IsPlayerHome);
            foreach (Map map in playerHomes)
            {
                if (OAFrame_MapUtility.GetSpecialBuildingManager(map)?.HasBuilding(OAGene_MiscDefOf.OAGene_HegemonicFlag) ?? false)
                {
                    return def.naturalGoodwillOffset;
                }
            }
        }
        return 0;
    }
}

