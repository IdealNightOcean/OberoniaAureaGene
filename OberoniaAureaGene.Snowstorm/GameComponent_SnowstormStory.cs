using OberoniaAurea_Frame;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_SnowstormStory : GameComponent
{
    protected bool storyActive;
    public bool StoryActive => storyActive;

    protected Pawn protagonist;
    public Pawn Protagonist => protagonist;

    public bool hometownSpawned;
    public bool storyInProgress;
    public bool storyFinished;

    public int hometownTile = Tile.Invalid;

    public bool LongingForHome => !storyInProgress && !storyFinished;

    public GameComponent_SnowstormStory(Game game) { }

    public void Notify_StoryActive()
    {
        storyActive = true;
        protagonist = (from p in Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount)
                       where p.IsColonist
                       select p).RandomElementWithFallback(null);
        protagonist?.health.GetOrAddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
        Log.Message(protagonist.NameShortColored);
    }

    public void Notify_HometownSpawned(int tile)
    {
        hometownSpawned = true;
        hometownTile = tile;
    }
    public void Notify_StoryInProgress()
    {
        storyInProgress = true;
        if (protagonist != null)
        {
            OAFrame_PawnUtility.RemoveFirstHediffOfDef(protagonist, Snowstrom_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
        }
    }

    public void Notify_StroyFailed()
    {
        storyInProgress = false;
        Hediff hediff = protagonist?.health.GetOrAddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
        hediff?.TryGetComp<HediffComp_ProtagonistHomecoming>()?.RecacheThought(forceNoLetter: true);
    }

    public override void LoadedGame()
    {
        base.LoadedGame();
        if (LongingForHome)
        {
            protagonist?.health.GetOrAddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
        }
    }


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref storyActive, "storyActive", defaultValue: false);
        Scribe_References.Look(ref protagonist, "protagonist");

        Scribe_Values.Look(ref hometownSpawned, "hometownSpawned", defaultValue: false);
        Scribe_Values.Look(ref storyInProgress, "storyInProgress", defaultValue: false);
        Scribe_Values.Look(ref storyFinished, "storyFinished", defaultValue: false);

        Scribe_Values.Look(ref hometownTile, "hometownTile", Tile.Invalid);
    }
}
