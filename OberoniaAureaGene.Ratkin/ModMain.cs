using AlienRace;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class OAGeneMod_Ratkin : Mod
{
    public static OAGene_RatkinSettings Settings;

    public OAGeneMod_Ratkin(ModContentPack content) : base(content)
    {
        Settings = GetSettings<OAGene_RatkinSettings>();
    }
    public override void DoSettingsWindowContents(Rect inRect) => Settings.DoSettingsWindowContents(inRect);

    public override string SettingsCategory() => "Mod.OberoniaAureaGene.Ratkin".Translate();
}


public class OAGene_RatkinSettings : ModSettings
{
    private Vector2 scrollPosition;
    private float viewRectHeight;

    private static bool EarUseHairColor = false;

    public void DoSettingsWindowContents(Rect inRect)
    {
        Rect outRect = new(inRect.x, inRect.y, inRect.width * 0.6f, inRect.height);
        outRect = outRect.CenteredOnXIn(inRect);
        float viewRectX = outRect.x + 8f;
        Rect viewRect = new(viewRectX, outRect.y, outRect.width - 16f, viewRectHeight);
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        Listing_Standard listing_Rect = new()
        {
            ColumnWidth = viewRect.width,
            maxOneColumn = true
        };
        listing_Rect.Begin(viewRect);

        listing_Rect.CheckboxLabeled($"OAGene_Setting_{nameof(EarUseHairColor)}".Translate(), ref EarUseHairColor);

        listing_Rect.End();
        if (Event.current.type == EventType.Layout)
        {
            viewRectHeight = listing_Rect.MaxColumnHeightSeen + 50f;
        }
        Widgets.EndScrollView();
    }

    private static void ResetEarColorChannel()
    {
        RaceSettings ratkin_RaceSetting = DefDatabase<AlienRace.RaceSettings>.GetNamed("RK_Race_Setting", errorOnFail: false);
        if (ratkin_RaceSetting is null)
            return;

        bool leftSetted = false;
        bool rightSetted = false;
        foreach (AlienPartGenerator.BodyAddon bodyAddons in ratkin_RaceSetting.universalBodyAddons)
        {
            if (!leftSetted && bodyAddons.path == "Things/Ratkin/BodyAddon/DummyEarLeft")
            {
                bodyAddons.ColorChannel = EarUseHairColor ? "hair" : "skin";
                leftSetted = true;
            }
            if (!rightSetted && bodyAddons.path == "Things/Ratkin/BodyAddon/DummyEarRight")
            {
                bodyAddons.ColorChannel = EarUseHairColor ? "hair" : "skin";
                rightSetted = true;
            }

            if (leftSetted && rightSetted)
                break;
        }
    }


    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref EarUseHairColor, nameof(EarUseHairColor), defaultValue: false);
        if (Scribe.mode == LoadSaveMode.Saving || Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            ResetEarColorChannel();
        }
    }
}