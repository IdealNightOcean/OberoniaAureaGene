using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_Precept_TerritorialConsciousnessSites : ThoughtWorker_Precept
{
    protected override ThoughtState ShouldHaveThought(Pawn p)
    {
        if (!p.Spawned || !p.Faction.IsPlayerFaction())
        {
            return ThoughtState.Inactive;
        }
        MapComponent_OberoniaAureaGene oaGene_MCOAG = p.Map?.GetOAGeneMapComp();
        if (oaGene_MCOAG == null)
        {
            return ThoughtState.Inactive;
        }
        if (oaGene_MCOAG.HasHostileSites)
        {
            return ThoughtState.ActiveDefault;
        }
        return ThoughtState.Inactive;
    }
}