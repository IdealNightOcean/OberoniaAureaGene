using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class RecipeWorker_RequireFinishStory : RecipeWorker
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        if (!Snowstorm_StoryUtility.StoryFinishedOnce())
        {
            return false;
        }
        return base.AvailableOnNow(thing, part);
    }
}
