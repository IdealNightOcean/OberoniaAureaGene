using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_Precept_TerritorialConsciousnessSites : ThoughtWorker_Precept
{
    protected override ThoughtState ShouldHaveThought(Pawn p)
    {
        if (!ModsConfig.IdeologyActive)
        {
            return ThoughtState.Inactive;
        }
        if (!p.Spawned || p.Faction != Faction.OfPlayer)
        {
            return ThoughtState.Inactive;
        }
        MapComponent_OberoniaAureaGene oaGene_MCOAG = p.Map?.GetComponent<MapComponent_OberoniaAureaGene>();
        if (oaGene_MCOAG == null)
        {
            return ThoughtState.Inactive;
        }
        if (oaGene_MCOAG.cachedHostileSitesCount > 0)
        {
            return ThoughtState.ActiveDefault;
        }
        return ThoughtState.Inactive;
    }
}