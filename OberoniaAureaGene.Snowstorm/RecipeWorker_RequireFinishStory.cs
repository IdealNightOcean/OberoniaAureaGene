using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class RecipeWorker_RequireFinishStory : RecipeWorker
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        if (!OAGene_SnowstormSettings.StoryFinishedOnce)
        {
            return false;
        }
        return base.AvailableOnNow(thing, part);
    }
}
