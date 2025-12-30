using Verse;
using Verse.AI;

namespace OberoniaAureaGene.Snowstorm;

public class MentalStateWorker_LostInMemory : MentalStateWorker
{
    public override bool StateCanOccur(Pawn pawn)
    {
        if (!base.StateCanOccur(pawn))
        {
            return false;
        }
        return Snowstorm_StoryUtility.IsStoryProtagonist(pawn);
    }
}
