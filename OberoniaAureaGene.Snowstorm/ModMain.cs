using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class OAGene_SnowstormMod : Mod
{
    public static OAGene_SnowstormSettings _settings;

    public OAGene_SnowstormMod(ModContentPack content) : base(content)
    {
        _settings = GetSettings<OAGene_SnowstormSettings>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        _settings.DoSettingsWindowContents(inRect);
        base.DoSettingsWindowContents(inRect);
    }
    public override void WriteSettings()
    {
        LoadedModManager.GetMod<OberoniaAureaGene_Mod>()?.WriteSettings();
        base.WriteSettings();
    }
    public override string SettingsCategory()
    {
        return "OAGene_SnowstormExpanded".Translate();
    }
}


[StaticConstructorOnStartup]
public class OAGene_SnowstormSettings : ModSettings
{
    private Vector2 scrollPosition;
    private float viewRectHeight;

    public static bool StoryFinishedOnce;


    public static bool SnowstormBreakDoor = true;

    public static bool IceStormBreakRoof = true;
    public static bool IceStormBreakNaturalRoof;
    public static bool IceStormBreakThickRoof;

    public static bool AllowSnowstormMaliciousRaid = true;
    public static bool AllowSnowstormMaliciousSite = true;

    public static bool ShowColumnProtectRadius = true;

    public static bool IceCrystalFlowerSpawnMessage;

    public static bool AllowDifficultEnemy = true;

    public void DoSettingsWindowContents(Rect inRect)
    {
        Rect outRect = new(inRect.x, inRect.y, inRect.width * 0.6f, inRect.height);
        outRect = outRect.CenteredOnXIn(inRect);
        float viewRectX = outRect.x + 8f;
        Rect viewRect = new(viewRectX, outRect.y, outRect.width - 16f, viewRectHeight);
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        Listing_Standard listing_Rect = new()
        {
            ColumnWidth = viewRect.width
        };
        listing_Rect.Begin(viewRect);

        listing_Rect.CheckboxLabeled("OAGene_SnowstormBreakRoof".Translate(), ref OberoniaAureaGene_Settings.SnowstormBreakRoof);
        if (OberoniaAureaGene_Settings.SnowstormBreakRoof)
        {
            listing_Rect.CheckboxLabeled("OAGene_SnowstormBreakNaturalRoof".Translate(), ref OberoniaAureaGene_Settings.SnowstormBreakNaturalRoof);
            listing_Rect.CheckboxLabeled("OAGene_SnowstormBreakThickRoof".Translate(), ref OberoniaAureaGene_Settings.SnowstormBreakThickRoof);
        }

        listing_Rect.Gap(6f);
        listing_Rect.CheckboxLabeled("OAGene_SnowstormBreakDoor".Translate(), ref SnowstormBreakDoor);

        listing_Rect.Gap(6f);
        listing_Rect.CheckboxLabeled("OAGene_IceStormBreakRoof".Translate(), ref IceStormBreakRoof);
        if (IceStormBreakRoof)
        {
            listing_Rect.CheckboxLabeled("OAGene_IceStormBreakNaturalRoof".Translate(), ref IceStormBreakNaturalRoof);
            listing_Rect.CheckboxLabeled("OAGene_IceStormBreakThickRoof".Translate(), ref IceStormBreakThickRoof);
        }

        listing_Rect.Gap(6f);
        listing_Rect.CheckboxLabeled("OAGene_ShowColumnProtectRadius".Translate(), ref ShowColumnProtectRadius);

        listing_Rect.Gap(6f);
        listing_Rect.CheckboxLabeled("OAGene_AllowSnowstormMaliciousRaid".Translate(), ref AllowSnowstormMaliciousRaid);
        listing_Rect.CheckboxLabeled("OAGene_AllowSnowstormMaliciousSite".Translate(), ref AllowSnowstormMaliciousSite);

        listing_Rect.Gap(6f);
        listing_Rect.CheckboxLabeled("OAGene_IceCrystalFlowerSpawnMessage".Translate(), ref IceCrystalFlowerSpawnMessage);

        listing_Rect.Gap(6f);
        listing_Rect.CheckboxLabeled("OAGene_AllowDifficultEnemy".Translate(), ref AllowDifficultEnemy);

        if (DebugSettings.ShowDevGizmos)
        {
            listing_Rect.Gap(12f);
            OberoniaAureaGene_Settings.ColumnProtectRadiusInt = (int)listing_Rect.SliderLabeled("OAGene_ColumnProtectRadius".Translate(OberoniaAureaGene_Settings.ColumnProtectRadiusInt.ToString()), OberoniaAureaGene_Settings.ColumnProtectRadiusInt, 1f, 7f);
            OberoniaAureaGene_Settings.ColumnProtectRadius = OberoniaAureaGene_Settings.ColumnProtectRadiusInt - 0.1f;

            listing_Rect.CheckboxLabeled("DEV: Story Finished", ref StoryFinishedOnce);
        }

        listing_Rect.Gap(12f);
        if (listing_Rect.ButtonText("OAGene_ForMountaintopCave".Translate()))
        {
            ForMountaintopCave();
        }
        listing_Rect.Gap(6f);
        if (listing_Rect.ButtonText("OAFrame_Reset".Translate()))
        {
            Reset();
        }
        listing_Rect.End();
        if (Event.current.type == EventType.Layout)
        {
            viewRectHeight = listing_Rect.MaxColumnHeightSeen + 100f;
        }
        Widgets.EndScrollView();
    }

    protected static void ForMountaintopCave()
    {
        OberoniaAureaGene_Settings.SnowstormForMountaintopCave();

        SnowstormBreakDoor = true;

        IceStormBreakRoof = true;
        IceStormBreakNaturalRoof = true;
        IceStormBreakThickRoof = true;

        AllowSnowstormMaliciousRaid = true;
        AllowSnowstormMaliciousSite = true;

        AllowDifficultEnemy = true;
    }
    protected static void Reset()
    {
        OberoniaAureaGene_Settings.SnowstormReset();

        SnowstormBreakDoor = true;

        IceStormBreakRoof = true;
        IceStormBreakNaturalRoof = false;
        IceStormBreakThickRoof = false;

        ShowColumnProtectRadius = true;

        AllowSnowstormMaliciousRaid = true;
        AllowSnowstormMaliciousSite = true;

        IceCrystalFlowerSpawnMessage = false;

        AllowDifficultEnemy = true;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref StoryFinishedOnce, "StoryFinishedOnce", defaultValue: false, forceSave: true);

        Scribe_Values.Look(ref SnowstormBreakDoor, "SnowstormBreakDoor", defaultValue: true);

        Scribe_Values.Look(ref IceStormBreakRoof, "IceStormBreakRoof", defaultValue: true);
        Scribe_Values.Look(ref IceStormBreakNaturalRoof, "IceStormBreakNaturalRoof", defaultValue: false);
        Scribe_Values.Look(ref IceStormBreakThickRoof, "IceStormBreakThickRoof", defaultValue: false);

        Scribe_Values.Look(ref ShowColumnProtectRadius, "ShowColumnProtectRadius", defaultValue: true);

        Scribe_Values.Look(ref AllowSnowstormMaliciousRaid, "AllowSnowstormMaliciousRaid", defaultValue: true);
        Scribe_Values.Look(ref AllowSnowstormMaliciousSite, "AllowSnowstormMaliciousSite", defaultValue: true);

        Scribe_Values.Look(ref IceCrystalFlowerSpawnMessage, "IceCrystalFlowerSpawnMessage", defaultValue: false);

        Scribe_Values.Look(ref AllowDifficultEnemy, "AllowDifficultEnemy", defaultValue: false);
    }
}