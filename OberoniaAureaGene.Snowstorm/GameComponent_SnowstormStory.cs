using OberoniaAurea_Frame;
using RimWorld.Planet;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class GameComponent_SnowstormStory : GameComponent
{
    private const float ScreenFadeSeconds = 15f;

    [Unsaved]
    protected float timeLeft = -1f;

    protected bool storyActive;
    public bool StoryActive => storyActive;

    protected Pawn protagonist;
    public Pawn Protagonist => protagonist;

    protected bool showNoProtagonistWarning;

    public bool hometownSpawned;
    public bool storyInProgress;
    public bool storyFinished;
    public bool LongingForHome => !storyInProgress && !storyFinished;

    public MapParent hometown;
    public Map hometownMap;
    public int hometownTile = Tile.Invalid;
    public bool satisfySnowstormCultist;


    public GameComponent_SnowstormStory(Game game) { }

    public void Notify_StoryActive()
    {
        storyActive = true;
        TrySetStoryProtagonist();
    }

    public bool TrySetStoryProtagonist()
    {
        protagonist = (from p in Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount)
                       where p.IsColonist
                       select p).RandomElementWithFallback(null);

        if (protagonist == null)
        {
            if (showNoProtagonistWarning)
            {
                Dialog_NodeTree failNodeTree = OAFrame_DiaUtility.ConfirmDiaNodeTree("OAGene_ResetProtagonistFail".Translate(),
                    "Confirm".Translate(),
                    null,
                    "OAFrame_DonotShowAgain".Translate(),
                    delegate
                    {
                        showNoProtagonistWarning = false;
                    });
                Find.WindowStack.Add(failNodeTree);
            }
            return false;
        }
        if (LongingForHome)
        {
            protagonist.health.GetOrAddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
        }
        Log.Message(protagonist.NameShortColored);
        return true;
    }

    public void Notify_HometownSpawned(MapParent mapParent, int tile)
    {
        hometownSpawned = true;
        hometown = mapParent;
        hometownTile = tile;
    }
    public void Notify_HometownDestory(bool clearTile = false)
    {
        hometownSpawned = false;
        hometown = null;
        if (clearTile)
        {
            hometownTile = Tile.Invalid;
        }
    }
    public void Notify_StoryInProgress()
    {
        storyInProgress = true;
        satisfySnowstormCultist = false;
        if (protagonist != null)
        {
            OAFrame_PawnUtility.RemoveFirstHediffOfDef(protagonist, Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
            protagonist.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecomed);
        }
    }
    public void Notify_StroyFail()
    {
        storyInProgress = false;
        if (protagonist != null)
        {
            OAFrame_PawnUtility.RemoveFirstHediffOfDef(protagonist, Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecomed);
            Hediff homecoming = protagonist.health.GetOrAddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
            homecoming?.TryGetComp<HediffComp_ProtagonistHomecoming>()?.RecacheDiaryAndThoughtNow(slience: true);
        }

    }
    public void Notify_StroySuccess()
    {
        if (!Find.TickManager.Paused)
        {
            Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
        }
        Find.TickManager.slower.SignalForceNormalSpeed();
        Find.MusicManagerPlay.ForcePlaySong(Snowstorm_MiscDefOf.OAGene_IGiorni, true);
        OAGene_SnowstormSettings.StoryFinishedOnce = true;
        storyFinished = true;
        storyInProgress = false;
        if (protagonist != null)
        {
            OAFrame_PawnUtility.RemoveFirstHediffOfDef(protagonist, Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecomed);
            OAFrame_PawnUtility.RemoveFirstHediffOfDef(protagonist, Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
        }
        ScreenFader.StartFade(Color.white, ScreenFadeSeconds);
        timeLeft = ScreenFadeSeconds;
    }

    public override void GameComponentUpdate()
    {
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                Snowstorm_StoryUtility.EndGame(protagonist);
            }
        }
    }

    public override void LoadedGame()
    {
        if (!storyActive)
        {
            return;
        }

        if (protagonist == null)
        {
            TrySetStoryProtagonist();
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref storyActive, "storyActive", defaultValue: false, forceSave: true);
        Scribe_References.Look(ref protagonist, "protagonist", saveDestroyedThings: true);
        Scribe_Values.Look(ref showNoProtagonistWarning, "showNoProtagonistWarning", defaultValue: true);

        Scribe_Values.Look(ref hometownSpawned, "hometownSpawned", defaultValue: false);
        Scribe_Values.Look(ref storyInProgress, "storyInProgress", defaultValue: false);
        Scribe_Values.Look(ref storyFinished, "storyFinished", defaultValue: false);

        Scribe_References.Look(ref hometown, "hometown");
        Scribe_References.Look(ref hometownMap, "hometownMap");
        Scribe_Values.Look(ref hometownTile, "hometownTile", Tile.Invalid);
        Scribe_Values.Look(ref satisfySnowstormCultist, "satisfySnowstormCultist", defaultValue: false);
    }
}
