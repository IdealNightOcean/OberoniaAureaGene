using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

//王权本位
public class ConditionalStatAffecter_MonarchyBased : ConditionalStatAffecter
{
    [MustTranslate]
    public string label;

    public override string Label => label;

    public override bool Applies(StatRequest req)
    {
        if (!req.HasThing || req.Thing is not Pawn p)
        {
            return false;
        }
        if (ModsConfig.RoyaltyActive)
        {
            if (p.royalty != null && p.royalty.AllTitlesForReading.Any())
            {
                return true;
            }
        }
        if (ModsConfig.IdeologyActive)
        {
            Precept_Role precept_Role = p.Ideo?.GetRole(p);
            if (precept_Role != null)
            {
                return true;
            }
        }
        return false;
    }
}
