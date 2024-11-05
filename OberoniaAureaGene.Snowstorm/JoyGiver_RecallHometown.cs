using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class JoyGiver_RecallHometown : JoyGiver_InPrivateRoom
{
    public override bool CanBeGivenTo(Pawn pawn)
    {
        if (!Snowstorm_StoryUtility.IsStoryProtagonist(pawn))
        {
            return false;
        }
        return base.CanBeGivenTo(pawn);
    }
}
