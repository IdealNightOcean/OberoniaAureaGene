using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class OberoniaAureaGene_Mod : Mod
{
    public static OberoniaAureaGene_Settings Settings;

    public OberoniaAureaGene_Mod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<OberoniaAureaGene_Settings>();
    }

    public override void DoSettingsWindowContents(Rect inRect) => Settings.DoSettingsWindowContents(inRect);

    public override string SettingsCategory() => "OberoniaAureaGene".Translate();
}

[StaticConstructorOnStartup]
public class OberoniaAureaGene_Settings : ModSettings
{
    public static bool SnowstormBreakRoof = true;
    public static bool SnowstormBreakNaturalRoof;
    public static bool SnowstormBreakThickRoof;

    public static float ColumnProtectRadius = 3.9f;
    public static float ColumnProtectRadiusInt = 4.0f;

    public static bool DodgeChancePatch = true;

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

            if (DebugSettings.ShowDevGizmos)
            {
                ColumnProtectRadiusInt = (int)listing_Rect.SliderLabeled("OAGene_ColumnProtectRadius".Translate(ColumnProtectRadiusInt.ToString()), ColumnProtectRadiusInt, 1f, 7f);
                ColumnProtectRadius = ColumnProtectRadiusInt - 0.1f;
            }
        }

        listing_Rect.Gap(6f);
        Text.Font = GameFont.Medium;
        listing_Rect.Label("OAGene_NeedRestart".Translate().Colorize(Color.red));
        Text.Font = GameFont.Small;
        listing_Rect.Gap(6f);
        listing_Rect.CheckboxLabeled("OAGene_DodgeChancePatch".Translate(), ref DodgeChancePatch, "OAGene_DodgeChancePatchTooltip".Translate());

        listing_Rect.GapLine(12f);
        if (listing_Rect.ButtonText("OAGene_ForMountaintopCave".Translate()))
        {
            SnowstormForMountaintopCave();
        }

        listing_Rect.Gap(6f);
        if (listing_Rect.ButtonText("OAFrame_Reset".Translate()))
        {
            Reset();
        }

        listing_Rect.End();

    }

    public static void SnowstormForMountaintopCave()
    {
        SnowstormBreakRoof = true;
        SnowstormBreakNaturalRoof = true;
        SnowstormBreakThickRoof = true;
    }

    protected static void Reset()
    {
        SnowstormReset();
        DodgeChancePatch = true;
    }

    public static void SnowstormReset()
    {
        SnowstormBreakRoof = true;
        SnowstormBreakNaturalRoof = false;
        SnowstormBreakThickRoof = false;

        ColumnProtectRadiusInt = 4.0f;
        ColumnProtectRadius = 3.9f;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref SnowstormBreakRoof, nameof(SnowstormBreakRoof), defaultValue: true);
        Scribe_Values.Look(ref SnowstormBreakNaturalRoof, nameof(SnowstormBreakNaturalRoof), defaultValue: false);
        Scribe_Values.Look(ref SnowstormBreakThickRoof, nameof(SnowstormBreakThickRoof), defaultValue: false);
        Scribe_Values.Look(ref ColumnProtectRadiusInt, nameof(ColumnProtectRadiusInt), 4.0f);
        Scribe_Values.Look(ref ColumnProtectRadius, nameof(ColumnProtectRadius), 3.9f);

        Scribe_Values.Look(ref DodgeChancePatch, nameof(DodgeChancePatch), defaultValue: true);
    }
}