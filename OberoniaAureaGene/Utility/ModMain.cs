using HarmonyLib;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class OberoniaAureaGene_Mod : Mod
{
    public static OberoniaAureaGene_Settings _settings;
    public static Harmony HarmonyInstance;

    public OberoniaAureaGene_Mod(ModContentPack content) : base(content)
    {
        _settings = GetSettings<OberoniaAureaGene_Settings>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        _settings.DoSettingsWindowContents(inRect);
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "OberoniaAureaGene".Translate();
    }
}

[StaticConstructorOnStartup]
public class OberoniaAureaGene_Settings : ModSettings
{
    public static bool SnowstormBreakRoof = true;
    public static bool SnowstormBreakNaturalRoof;
    public static bool SnowstormBreakThickRoof;
    public void DoSettingsWindowContents(Rect inRect)
    {
        Rect viewRect = new(inRect.x, inRect.y, inRect.width * 0.6f, inRect.height);

        Listing_Standard listing_Rect = new()
        {
            ColumnWidth = viewRect.width
        };
        listing_Rect.Begin(viewRect);

        listing_Rect.CheckboxLabeled("OAGene_SnowstormBreakRoof".Translate(), ref SnowstormBreakRoof);
        if (SnowstormBreakRoof)
        {
            listing_Rect.CheckboxLabeled("OAGene_SnowstormBreakNaturalRoof".Translate(), ref SnowstormBreakNaturalRoof);
            listing_Rect.CheckboxLabeled("OAGene_SnowstormBreakThickRoof".Translate(), ref SnowstormBreakThickRoof);
        }

        listing_Rect.Gap(6f);
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

    }

    protected void ForMountaintopCave()
    {
        SnowstormBreakRoof = true;
        SnowstormBreakNaturalRoof = true;
        SnowstormBreakThickRoof = true;
    }

    protected void Reset()
    {
        SnowstormBreakRoof = true;
        SnowstormBreakNaturalRoof = false;
        SnowstormBreakThickRoof = false;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref SnowstormBreakRoof, "SnowstormBreakRoof", defaultValue: true);
        Scribe_Values.Look(ref SnowstormBreakNaturalRoof, "SnowstormBreakNaturalRoof", defaultValue: false);
        Scribe_Values.Look(ref SnowstormBreakThickRoof, "SnowstormBreakThickRoof", defaultValue: false);
    }
}