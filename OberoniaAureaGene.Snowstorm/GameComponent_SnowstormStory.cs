using OberoniaAurea_Frame;
using RimWorld.Planet;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_SnowstormStory : GameComponent
{
    private const float ScreenFadeSeconds = 6f;
    private const float SongStartDelay = 2.5f;
    [Unsaved]
    protected float timeLeft = -1f;
    [Unsaved]
    protected bool onlyProtagonist = false;

    protected bool storyActive;
    public bool StoryActive => storyActive;

    protected Pawn protagonist;
    public Pawn Protagonist => protagonist;

    public bool hometownSpawned;
    public bool storyInProgress;
    public bool storyFinished;
    public bool LongingForHome => !storyInProgress && !storyFinished;

    public int hometownTile = Tile.Invalid;


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

    public void Notify_StroyFail()
    {
        storyInProgress = false;
        Hediff hediff = protagonist?.health.GetOrAddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
        hediff?.TryGetComp<HediffComp_ProtagonistHomecoming>()?.RecacheDiaryAndThoughtNow(slience: true);
    }

    public void Notify_StroySuccess(bool onlyProtagonist)
    {
        if (!Find.TickManager.Paused)
        {
            Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
        }
        OAGene_SnowstormSettings.StoryFinishedOnce = true;
        storyFinished = true;
        storyInProgress = false;
        this.onlyProtagonist = onlyProtagonist;

        ScreenFader.StartFade(Color.white, 6f);
        timeLeft = ScreenFadeSeconds;
    }
    public override void GameComponentUpdate()
    {
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                Snowstorm_StoryUtility.EndGame(protagonist, onlyProtagonist);
            }
        }
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
