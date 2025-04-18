﻿using RimWorld;
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
                MapComponent_OberoniaAureaGene oaGene_MCOAG = map.OAGeneMapComp();
                if (oaGene_MCOAG != null && oaGene_MCOAG.HasHegemonicFlag)
                {
                    return def.naturalGoodwillOffset;
                }
            }
        }
        return 0;
    }
}

