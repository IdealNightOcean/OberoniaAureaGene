using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_SnowstormStory : GameComponent
{
    protected bool storyActive;
    public bool StoryActive => storyActive;

    protected Pawn protagonist;
    public Pawn Protagonist => protagonist;

    protected bool storyStart;
    public bool StoryStart => storyStart;

    protected bool storyFinishedOnce;
    public bool StoryFinishedOnce => storyFinishedOnce;


    public GameComponent_SnowstormStory(Game game) { }

    public void Notify_StoryActive()
    {
        storyActive = true;
        protagonist = (from p in Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount)
                       where p.IsColonist
                       select p).RandomElementWithFallback(null);
        Log.Message(protagonist.NameShortColored);
    }
    public void Notify_StoryStart()
    {
        storyStart = true;
        if (protagonist != null)
        {

        }
    }

    public override void LoadedGame()
    {
        base.LoadedGame();
        if (!storyStart && protagonist != null)
        {
            float days = Find.TickManager.TicksGame.TicksToDays();
        }
    }


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref storyActive, "storyActive", defaultValue: false);
        Scribe_References.Look(ref protagonist, "protagonist");

        Scribe_Values.Look(ref storyStart, "storyStart", defaultValue: false);
        Scribe_Values.Look(ref storyFinishedOnce, "storyFinishedOnce", defaultValue: false);
    }
}
