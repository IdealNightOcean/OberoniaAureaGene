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

    public static bool IceCrystalFlowerSpawnMessage;

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
        listing_Rect.CheckboxLabeled("OAGene_AllowSnowstormMaliciousRaid".Translate(), ref AllowSnowstormMaliciousRaid);
        listing_Rect.CheckboxLabeled("OAGene_AllowSnowstormMaliciousSite".Translate(), ref AllowSnowstormMaliciousSite);

        listing_Rect.Gap(6f);
        listing_Rect.CheckboxLabeled("OAGene_IceCrystalFlowerSpawnMessage".Translate(), ref IceCrystalFlowerSpawnMessage);

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

    protected void ForMountaintopCave()
    {
        OberoniaAureaGene_Settings.SnowstormBreakRoof = true;
        OberoniaAureaGene_Settings.SnowstormBreakNaturalRoof = true;
        OberoniaAureaGene_Settings.SnowstormBreakThickRoof = true;
        SnowstormBreakDoor = true;

        IceStormBreakRoof = true;
        IceStormBreakNaturalRoof = true;
        IceStormBreakThickRoof = true;

        AllowSnowstormMaliciousRaid = true;
        AllowSnowstormMaliciousSite = true;

        IceCrystalFlowerSpawnMessage = false;
    }
    protected void Reset()
    {
        OberoniaAureaGene_Settings.SnowstormBreakRoof = true;
        OberoniaAureaGene_Settings.SnowstormBreakNaturalRoof = false;
        OberoniaAureaGene_Settings.SnowstormBreakThickRoof = false;
        SnowstormBreakDoor = true;

        IceStormBreakRoof = true;
        IceStormBreakNaturalRoof = false;
        IceStormBreakThickRoof = false;

        AllowSnowstormMaliciousRaid = true;
        AllowSnowstormMaliciousSite = true;

        IceCrystalFlowerSpawnMessage = false;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref StoryFinishedOnce, "StoryFinishedOnce", defaultValue: false);

        Scribe_Values.Look(ref SnowstormBreakDoor, "SnowstormBreakDoor", defaultValue: true);

        Scribe_Values.Look(ref IceStormBreakRoof, "IceStormBreakRoof", defaultValue: true);
        Scribe_Values.Look(ref IceStormBreakNaturalRoof, "IceStormBreakNaturalRoof", defaultValue: false);
        Scribe_Values.Look(ref IceStormBreakThickRoof, "IceStormBreakThickRoof", defaultValue: false);

        Scribe_Values.Look(ref AllowSnowstormMaliciousRaid, "AllowSnowstormMaliciousRaid", defaultValue: true);
        Scribe_Values.Look(ref AllowSnowstormMaliciousSite, "AllowSnowstormMaliciousSite", defaultValue: true);

        Scribe_Values.Look(ref IceCrystalFlowerSpawnMessage, "IceCrystalFlowerSpawnMessage", defaultValue: false);
    }
}