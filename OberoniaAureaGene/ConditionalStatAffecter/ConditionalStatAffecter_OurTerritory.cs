
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ConditionalStatAffecter_OurTerritory : ConditionalStatAffecter
{
    [MustTranslate]
    public string label;
    public override string Label => label;

    public override bool Applies(StatRequest req)
    {
        if (req.HasThing && req.Thing is Pawn pawn)
        {
            return pawn.MapHeld?.IsPlayerHome ?? false;
        }
        return false;
    }
}
