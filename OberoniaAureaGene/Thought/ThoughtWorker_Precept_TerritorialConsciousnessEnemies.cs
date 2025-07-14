using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_Precept_TerritorialConsciousnessEnemies : ThoughtWorker_Precept
{
    protected override ThoughtState ShouldHaveThought(Pawn p)
    {
        if (!p.Spawned || !p.Faction.IsPlayerSafe())
        {
            return ThoughtState.Inactive;
        }
        MapComponent_OberoniaAureaGene oaGene_MCOAG = p.Map.OAGeneMapComp();
        if (oaGene_MCOAG is null)
        {
            return ThoughtState.Inactive;
        }
        int enemiesCount = oaGene_MCOAG.cachedEnemiesCount;
        if (enemiesCount > 30)
        {
            return ThoughtState.ActiveAtStage(2);
        }
        else if (enemiesCount > 5)
        {
            return ThoughtState.ActiveAtStage(1);
        }
        else if (enemiesCount > 0)
        {
            return ThoughtState.ActiveAtStage(0);
        }
        return ThoughtState.Inactive;
    }
}
