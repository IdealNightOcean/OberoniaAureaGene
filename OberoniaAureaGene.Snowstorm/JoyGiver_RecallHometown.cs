using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class JoyGiver_RecallHometown : JoyGiver_Skygaze
{
    public override bool CanBeGivenTo(Pawn pawn)
    {
        if (!base.CanBeGivenTo(pawn))
        {
            return false;
        }
        return Snowstorm_StoryUtility.IsStoryProtagonist(pawn);
    }
}
