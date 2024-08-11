using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

public class GoodwillSituationWorker_HasHegemonicFlag : GoodwillSituationWorker
{
    public override int GetNaturalGoodwillOffset(Faction other)
    {
        if (!ModsConfig.IdeologyActive)
        {
            return 0;
        }
        var playerHomes = Find.Maps.Where(m => m.IsPlayerHome);
        foreach (Map map in playerHomes)
        {
            MapComponent_OberoniaAureaGene oaGene_MCOAG = map.GetOAGeneMapComp();
            if (oaGene_MCOAG != null && oaGene_MCOAG.HasHegemonicFlag)
            {
                return def.naturalGoodwillOffset;
            }
        }
        return 0;
    }
}

