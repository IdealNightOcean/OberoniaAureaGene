using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_Precept_HasHegemonicFlag : ThoughtWorker_Precept
{
    protected override ThoughtState ShouldHaveThought(Pawn p)
    {
        if (!p.Spawned || p.Faction != Faction.OfPlayer)
        {
            return ThoughtState.Inactive;
        }
        MapComponent_OberoniaAureaGene oaGene_MCOAG = p.Map?.GetComponent<MapComponent_OberoniaAureaGene>();
        if (oaGene_MCOAG == null)
        {
            return ThoughtState.Inactive;
        }
        if (oaGene_MCOAG.HasHegemonicFlag)
        {
            return ThoughtState.ActiveDefault;
        }
        return ThoughtState.Inactive;
    }

}
