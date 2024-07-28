using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ThoughtWorker_MonarchyBased : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (ModsConfig.RoyaltyActive)
        {
            if (p.royalty != null && p.royalty.AllTitlesForReading.Any())
            {
                return ThoughtState.Inactive;
            }
        }
        if (ModsConfig.IdeologyActive)
        {
            Precept_Role precept_Role = p.Ideo?.GetRole(p);
            if (precept_Role != null)
            {
                return ThoughtState.Inactive;
            }
        }
        return ThoughtState.ActiveDefault;
    }
}
