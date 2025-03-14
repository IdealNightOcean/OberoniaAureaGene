﻿using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_Precept_HasHegemonicFlag : ThoughtWorker_Precept
{
    protected override ThoughtState ShouldHaveThought(Pawn p)
    {
        if (!p.Spawned || !p.Faction.IsPlayerFaction())
        {
            return ThoughtState.Inactive;
        }
        MapComponent_OberoniaAureaGene oaGene_MCOAG = p.Map?.OAGeneMapComp();
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
