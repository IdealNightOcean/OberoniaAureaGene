using OberoniaAurea_Frame;
using RimWorld.Planet;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_SnowstormStory : GameComponent
{
    public static GameComponent_SnowstormStory Instance { get; private set; }

    private const float ScreenFadeSeconds = 15f;

    [Unsaved] protected float timeLeft = -1f;

    protected bool storyActive;
    public bool StoryActive => storyActive;

    protected Pawn protagonist;
    public Pawn Protagonist => protagonist;

    protected bool showNoProtagonistWarning = true;

    public bool storyInProgress;
    public bool storyFinished;
    public bool LongingForHome => !storyInProgress && !storyFinished;

    public bool satisfySnowstormCultist;

    public bool hometownSpawned;
    public MapParent hometown;
    public Map hometownMap;
    public int hometownTile = Tile.Invalid;

    public GameComponent_SnowstormStory(Game game) => Instance = this;
    public static void OpenDevWindow() => Find.WindowStack.Add(new DevWin_SnowstormStory());

    public void DrawDevWindow(Listing_Standard listing_Rect)
    {
        Text.Font = GameFont.Medium;
        listing_Rect.Label($"风雪遗孤故事是否开启: {storyActive}");
        Text.Font = GameFont.Small;
        listing_Rect.Gap(3f);
        listing_Rect.Label($"归乡任务是否进行中: {storyInProgress}");
        listing_Rect.Label($"归乡任务是否已完成: {storyFinished}");
        listing_Rect.Label($"是否显式无遗孤主角警告: {showNoProtagonistWarning}");
        listing_Rect.Gap(3f);

        if (protagonist is null) { listing_Rect.Label("Protagonist: None"); }
        else { listing_Rect.Label($"遗孤主角: {protagonist}"); }

        listing_Rect.Label($"主角当前是否渴望归乡: {LongingForHome}");
        listing_Rect.Gap(3f);
        listing_Rect.Label($"是否满足了归乡任务的风雪教徒: {satisfySnowstormCultist}");
        listing_Rect.Gap(3f);

        listing_Rect.Label($"家乡是否已生成: {hometownSpawned}");
        if (hometown is null) { listing_Rect.Label("家乡: 无"); }
        else { listing_Rect.Label($"家乡: {hometown}"); }

        if (hometownMap is null) { listing_Rect.Label("家乡地图: 无"); }
        else { listing_Rect.Label($"家乡地图: {hometownMap}"); }

        listing_Rect.Label($"家乡Tile: {hometownTile}");
    }

    public void Notify_StoryActive()
    {
        storyActive = true;
        Log.Message("[OAGene] OAGene_Log_SnowstormStoryActive".Translate().Colorize(Color.green));
        protagonist = Find.GameInitData.startingAndOptionalPawns.Where(p => p.IsColonist).Take(Find.GameInitData.startingPawnCount).RandomElementWithFallback(null);
        ProtagonistValidator();
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
        if (protagonist is not null)
        {
            protagonist.RemoveFirstHediffOfDef(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
            protagonist.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecomed);
        }
    }

    public void Notify_StroyFail()
    {
        storyInProgress = false;
        if (protagonist is not null)
        {
            protagonist.RemoveFirstHediffOfDef(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecomed);
            Hediff_ProtagonistHomecoming homecoming = (Hediff_ProtagonistHomecoming)protagonist.health.GetOrAddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
            homecoming?.RecacheDiaryAndThoughtNow(slience: true);
        }
    }

    public void Notify_StroySuccess()
    {
        OAGene_SnowstormSettings.StoryFinishedOnce = true;
        try
        {
            LoadedModManager.GetMod<OAGene_SnowstormMod>()?.WriteSettings();
        }
        catch
        {
            Log.Error("[OAGene] OAGene_SnowstormMod write setting failed.");
        }
        storyFinished = true;
        storyInProgress = false;
        if (protagonist is not null)
        {
            protagonist.RemoveFirstHediffOfDef(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecomed);
            protagonist.RemoveFirstHediffOfDef(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
        }
        Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
        Find.TickManager.slower.SignalForceNormalSpeed();
        Find.MusicManagerPlay.ForcePlaySong(Snowstorm_MiscDefOf.OAGene_IGiorni, true);
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

    public override void StartedNewGame()
    {
        GameStart();
    }

    public override void LoadedGame()
    {
        GameStart();
    }

    private void GameStart()
    {
        if (!storyActive)
        {
            Log.Message("[OAGene] OAGene_Log_SnowstormStoryNotActive".Translate().Colorize(Color.gray));
            return;
        }

        Log.Message("[OAGene] OAGene_Log_SnowstormStoryActive".Translate().Colorize(Color.green));
        ProtagonistValidator();
    }

    private void ProtagonistValidator()
    {
        if (protagonist is not null)
        {
            if (LongingForHome)
            {
                protagonist.health.GetOrAddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
            }
            Log.Message("[OAGene] OAGene_Log_StoryProtagonist".Translate(protagonist.Named("PAWN")).Colorize(Color.green));
        }
        else
        {
            Log.Message("[OAGene] OAGene_Log_NoStoryProtagonist".Translate().Colorize(Color.red));
            if (showNoProtagonistWarning)
            {
                Log.Error("[OAGene] Snowstorm Story is active but lacks a definitive protagonist.");
                DiaResetProtagonist();
            }
        }
    }

    private void DiaResetProtagonist()
    {
        DiaNode rootNode = new("OAGene_HasNoProtagonist".Translate());
        DiaOption resetOpt = new("OAGene_ResetProtagonist".Translate())
        {
            resolveTree = true,
            action = delegate
            {
                bool forceReset = false;
                protagonist = Find.GameInitData?.startingAndOptionalPawns.Where(p => p.IsColonist)?.Take(Find.GameInitData.startingPawnCount).RandomElementWithFallback(null);
                if (protagonist is null)
                {
                    forceReset = true;
                    protagonist = Find.AnyPlayerHomeMap?.mapPawns.FreeColonistsSpawned.RandomElementWithFallback(null);
                }
                Dialog_NodeTree outcomeTree;
                if (protagonist is null)
                {
                    outcomeTree = OAFrame_DiaUtility.ConfirmDiaNodeTree("OAGene_ResetProtagonistFail".Translate(), "Confirm".Translate(), null, "OAFrame_DonotShowAgain".Translate(), delegate { showNoProtagonistWarning = false; });
                }
                else
                {
                    string successText = forceReset ? "OAGene_ResetProtagonistSuccessForce" : "OAGene_ResetProtagonistSuccess";
                    outcomeTree = OAFrame_DiaUtility.DefaultConfirmDiaNodeTree(successText.Translate(protagonist.Named("PAWN")));
                    if (LongingForHome)
                    {
                        protagonist.health.GetOrAddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_ProtagonistHomecoming);
                    }
                }
                Find.WindowStack.Add(outcomeTree);
            }
        };

        DiaOption backOpt = new("GoBack".Translate())
        {
            resolveTree = true,
        };

        DiaOption neverShowOpt = new("OAFrame_DonotShowAgain".Translate())
        {
            resolveTree = true,
            action = delegate
            {
                showNoProtagonistWarning = false;
            }
        };
        rootNode.options.Add(resetOpt);
        rootNode.options.Add(backOpt);
        rootNode.options.Add(neverShowOpt);
        Dialog_NodeTree failNodeTree = new(rootNode);
        Find.WindowStack.Add(failNodeTree);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref storyActive, "storyActive", defaultValue: false, forceSave: true);
        Scribe_References.Look(ref protagonist, "protagonist", saveDestroyedThings: true);
        Scribe_Values.Look(ref showNoProtagonistWarning, "showNoProtagonistWarning", defaultValue: true);

        Scribe_Values.Look(ref hometownSpawned, "hometownSpawned", defaultValue: false, forceSave: true);
        Scribe_Values.Look(ref storyInProgress, "storyInProgress", defaultValue: false, forceSave: true);
        Scribe_Values.Look(ref storyFinished, "storyFinished", defaultValue: false, forceSave: true);

        Scribe_References.Look(ref hometown, "hometown");
        Scribe_References.Look(ref hometownMap, "hometownMap");
        Scribe_Values.Look(ref hometownTile, "hometownTile", Tile.Invalid);
        Scribe_Values.Look(ref satisfySnowstormCultist, "satisfySnowstormCultist", defaultValue: false);
    }
}
