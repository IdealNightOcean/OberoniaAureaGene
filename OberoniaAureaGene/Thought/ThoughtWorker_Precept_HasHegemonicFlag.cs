using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_Precept_HasHegemonicFlag : ThoughtWorker_Precept
{
    protected override ThoughtState ShouldHaveThought(Pawn p)
    {
        if (!p.Spawned || !p.Faction.IsPlayer)
        {
            return ThoughtState.Inactive;
        }
        MapComponent_OberoniaAureaGene oaGene_MCOAG = p.Map?.GetOAGeneMapComp();
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
