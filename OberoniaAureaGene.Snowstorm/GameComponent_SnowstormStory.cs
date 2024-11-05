using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_SnowstormStory : GameComponent
{
    protected bool storyActive;
    public bool StoryActive => storyActive;

    protected Pawn protagonist;
    public Pawn Protagonist => protagonist;

    protected bool storyFinishedOnce;
    public bool StoryFinishedOnce => storyFinishedOnce;

    public GameComponent_SnowstormStory(Game game) { }

    public void Notify_StoryActive()
    {
        storyActive = true;
        protagonist = Find.GameInitData.startingAndOptionalPawns.Where(p => p.IsColonist).RandomElementWithFallback(null);
        Log.Message(protagonist.NameShortColored);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref storyActive, "storyActive", defaultValue: false);
        Scribe_References.Look(ref protagonist, "protagonist");

        Scribe_Values.Look(ref storyFinishedOnce, "storyFinishedOnce", defaultValue: false);
    }
}
