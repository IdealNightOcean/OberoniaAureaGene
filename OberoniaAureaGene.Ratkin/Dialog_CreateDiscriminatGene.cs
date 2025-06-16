using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OberoniaAureaGene.Ratkin;

public class Dialog_CreateDiscriminatGene : GeneCreationDialogBase
{
    private readonly Building_GeneDiscriminatorBase geneDiscriminator;

    private readonly List<Genepack> libraryGenepacks = [];

    private readonly List<Genepack> unpoweredGenepacks = [];

    private Genepack selectedGenepack;

    private GeneDef selectedGene;

    protected override List<GeneDef> SelectedGenes => [selectedGene];
    private readonly HashSet<Genepack> matchingGenepacks = [];

    private readonly Color UnpoweredColor = new(0.5f, 0.5f, 0.5f, 0.5f);

    private readonly List<GeneDef> tmpGenes = [];

    public override Vector2 InitialSize => new(1016f, UI.screenHeight);

    protected override string Header => "OAGene_DiscriminatGenes".Translate();

    protected override string AcceptButtonLabel => "OAGene_StartDiscriminating".Translate();


    public Dialog_CreateDiscriminatGene(Building_GeneDiscriminatorBase geneDiscriminator)
    {
        this.geneDiscriminator = geneDiscriminator;
        libraryGenepacks.AddRange(geneDiscriminator.GetGenepacks(includePowered: true, includeUnpowered: true));
        unpoweredGenepacks.AddRange(geneDiscriminator.GetGenepacks(includePowered: false, includeUnpowered: true));
        closeOnAccept = false;
        forcePause = true;
        absorbInputAroundWindow = true;
        searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
        libraryGenepacks.SortGenepacks();
        unpoweredGenepacks.SortGenepacks();
    }

    protected override void Accept()
    {
        if (geneDiscriminator.Working)
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("OAGene_ConfirmStartNewGeneDiscriminat".Translate(), StartDiscriminat, destructive: true));
        }
        else
        {
            StartDiscriminat();
        }
    }

    private void StartDiscriminat()
    {
        geneDiscriminator.TryStartWork(selectedGenepack, selectedGene);
        SoundDefOf.StartRecombining.PlayOneShotOnCamera();
        Close(doCloseSound: false);
    }
    public override void DoWindowContents(Rect rect)
    {
        Rect rect2 = rect;
        rect2.yMax -= ButSize.y + 4f;
        Rect rect3 = new(rect2.x, rect2.y, rect2.width, 35f);
        Text.Font = GameFont.Medium;
        Widgets.Label(rect3, Header);
        Text.Font = GameFont.Small;
        DrawSearchRect(rect);
        rect2.yMin += 39f;
        Rect rect4 = new(rect2.x + Margin, rect2.y, rect2.width - Margin * 2f, rect2.height - 8f);
        DrawGenes(rect4);
        Rect rect5 = rect;
        rect5.yMin = rect5.yMax - ButSize.y;
        DoBottomButtons(rect5);
    }
    protected override void DrawGenes(Rect rect)
    {
        GUI.BeginGroup(rect);
        Rect rect2 = new(0f, 0f, rect.width - 16f, scrollHeight);
        float curY = 0f;
        Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, rect2);
        Rect containingRect = rect2;
        containingRect.y = scrollPosition.y;
        containingRect.height = rect.height;
        DrawSelectSection(rect, selectedGenepack, "SelectedGenepacks".Translate(), ref curY, ref selectedHeight, containingRect);
        curY += 8f;
        DrawLibrarySection(rect, libraryGenepacks, "GenepackLibrary".Translate(), ref curY, ref unselectedHeight, containingRect);
        if (Event.current.type == EventType.Layout)
        {
            scrollHeight = curY;
        }
        Widgets.EndScrollView();
        GUI.EndGroup();
    }
    private void DrawSelectSection(Rect rect, Genepack genepack, string label, ref float curY, ref float sectionHeight, Rect containingRect)
    {
        float curX = 4f;
        Rect rect2 = new(10f, curY, rect.width - 16f - 10f, Text.LineHeight);
        Widgets.Label(rect2, label);

        Text.Anchor = TextAnchor.UpperRight;
        GUI.color = ColoredText.SubtleGrayColor;
        Widgets.Label(rect2, "ClickToAddOrRemove".Translate());
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;

        curY += Text.LineHeight + 3f;
        float num = curY;
        Rect rect3 = new(0f, curY, rect.width, sectionHeight);
        Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor);
        curY += 4f;
        if (genepack == null)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = ColoredText.SubtleGrayColor;
            Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        else
        {
            List<GeneDef> tmpSelectedGenes = selectedGenepack.GeneSet.GenesListForReading;
            for (int i = 0; i < tmpSelectedGenes.Count; i++)
            {
                GeneDef gene = tmpSelectedGenes[i];
                if (gene.biostatArc > 0)
                {
                    continue;
                }
                float num2 = 34f + GeneCreationDialogBase.GeneSize.x + 4f * (float)(genepack.GeneSet.GenesListForReading.Count + 2);
                if (curX + num2 > rect.width - 16f)
                {
                    curX = 4f;
                    curY += GeneCreationDialogBase.GeneSize.y + 8f + 14f;
                }
                if (selectedGene == gene)
                {
                    Widgets.DrawBox(new Rect(curX, curY, num2, GeneCreationDialogBase.GeneSize.y + 8f), 2);
                }
                if (DrawSelectedGene(gene, ref curX, curY, num2, containingRect))
                {
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                    selectedGene = gene;
                }
            }
        }
        curY += GeneCreationDialogBase.GeneSize.y + 12f;
        if (Event.current.type == EventType.Layout)
        {
            sectionHeight = curY - num;
        }
    }

    private void DrawLibrarySection(Rect rect, List<Genepack> genepacks, string label, ref float curY, ref float sectionHeight, Rect containingRect)
    {
        float curX = 4f;
        Rect rect2 = new(10f, curY, rect.width - 16f - 10f, Text.LineHeight);
        Widgets.Label(rect2, label);

        curY += Text.LineHeight + 3f;
        float num = curY;
        Rect rect3 = new(0f, curY, rect.width, sectionHeight);
        Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor);
        curY += 4f;
        if (!genepacks.Any())
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = ColoredText.SubtleGrayColor;
            Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        else
        {
            for (int i = 0; i < genepacks.Count; i++)
            {
                Genepack genepack = genepacks[i];
                if (genepack.GeneSet.GenesListForReading.Count < 2)
                {
                    continue;
                }
                if (quickSearchWidget.filter.Active && !matchingGenepacks.Contains(genepack))
                {
                    continue;
                }
                float num2 = 34f + GeneCreationDialogBase.GeneSize.x * (float)genepack.GeneSet.GenesListForReading.Count + 4f * (float)(genepack.GeneSet.GenesListForReading.Count + 2);
                if (curX + num2 > rect.width - 16f)
                {
                    curX = 4f;
                    curY += GeneCreationDialogBase.GeneSize.y + 8f + 14f;
                }
                if (selectedGenepack == genepack)
                {
                    Widgets.DrawBox(new Rect(curX, curY, num2, GeneCreationDialogBase.GeneSize.y + 8f), 2);
                }
                if (DrawGenepack(genepack, ref curX, curY, num2, containingRect))
                {
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                    if (selectedGenepack == genepack)
                    {
                        selectedGenepack = null;
                    }
                    else
                    {
                        selectedGenepack = genepack;
                    }
                    selectedGene = null;
                    break;
                }
            }
        }
        curY += GeneCreationDialogBase.GeneSize.y + 12f;
        if (Event.current.type == EventType.Layout)
        {
            sectionHeight = curY - num;
        }
    }

    private bool DrawGenepack(Genepack genepack, ref float curX, float curY, float packWidth, Rect containingRect)
    {
        bool result = false;
        if (genepack.GeneSet == null || genepack.GeneSet.GenesListForReading.NullOrEmpty())
        {
            return result;
        }
        Rect rect = new(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
        if (!containingRect.Overlaps(rect))
        {
            curX = rect.xMax + 14f;
            return false;
        }
        Widgets.DrawHighlight(rect);
        GUI.color = GeneCreationDialogBase.OutlineColorUnselected;
        Widgets.DrawBox(rect);
        GUI.color = Color.white;
        curX += 4f;
        GeneUIUtility.DrawBiostats(genepack.GeneSet.ComplexityTotal, genepack.GeneSet.MetabolismTotal, genepack.GeneSet.ArchitesTotal, ref curX, curY, 4f);
        List<GeneDef> genesListForReading = genepack.GeneSet.GenesListForReading;
        for (int i = 0; i < genesListForReading.Count; i++)
        {
            GeneDef gene = genesListForReading[i];
            if (quickSearchWidget.filter.Active && matchingGenes.Contains(gene))
            {
                matchingGenepacks.Contains(genepack);
            }
            else
                _ = 0;
            bool overridden = leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(gene));
            Rect geneRect = new(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
            string extraTooltip = null;
            if (leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == gene))
            {
                extraTooltip = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == gene));
            }
            else if (cachedOverriddenGenes.Contains(gene))
            {
                extraTooltip = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(gene)));
            }
            else if (randomChosenGroups.ContainsKey(gene))
            {
                extraTooltip = ("GeneWillBeRandomChosen".Translate() + ":\n" + randomChosenGroups[gene].Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
            }
            GeneUIUtility.DrawGeneDef(genesListForReading[i], geneRect, GeneType.Xenogene, () => extraTooltip, doBackground: false, clickable: false, overridden);
            curX += GeneCreationDialogBase.GeneSize.x + 4f;
        }
        Widgets.InfoCardButton(rect.xMax - 24f, rect.y + 2f, genepack);
        if (unpoweredGenepacks.Contains(genepack))
        {
            Widgets.DrawBoxSolid(rect, UnpoweredColor);
            TooltipHandler.TipRegion(rect, "GenepackUnusableGenebankUnpowered".Translate().Colorize(ColorLibrary.RedReadable));
        }
        if (Mouse.IsOver(rect))
        {
            Widgets.DrawHighlight(rect);
        }
        if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect) && Event.current.button == 1)
        {
            List<FloatMenuOption> list = [];
            list.Add(new FloatMenuOption("EjectGenepackFromGeneBank".Translate(), delegate
            {
                CompGenepackContainer geneBankHoldingPack = geneDiscriminator.GetGeneBankHoldingPack(genepack);
                if (geneBankHoldingPack != null)
                {
                    ThingWithComps parent = geneBankHoldingPack.parent;
                    if (geneBankHoldingPack.innerContainer.TryDrop(genepack, parent.def.hasInteractionCell ? parent.InteractionCell : parent.Position, parent.Map, ThingPlaceMode.Near, 1, out var _))
                    {
                        if (selectedGenepack == genepack)
                        {
                            selectedGenepack = null;
                            selectedGene = null;
                        }
                        tmpGenes.Clear();
                        libraryGenepacks.Clear();
                        unpoweredGenepacks.Clear();
                        matchingGenepacks.Clear();
                        libraryGenepacks.AddRange(geneDiscriminator.GetGenepacks(includePowered: true, includeUnpowered: true));
                        unpoweredGenepacks.AddRange(geneDiscriminator.GetGenepacks(includePowered: false, includeUnpowered: true));
                        libraryGenepacks.SortGenepacks();
                        unpoweredGenepacks.SortGenepacks();
                    }
                }
            }));
            Find.WindowStack.Add(new FloatMenu(list));
        }
        else if (Widgets.ButtonInvisible(rect))
        {
            result = true;
        }
        curX = Mathf.Max(curX, rect.xMax + 14f);
        return result;
        static string GroupInfo(GeneLeftChosenGroup group)
        {
            if (group == null)
            {
                return null;
            }
            return ("GeneOneActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + group.overriddenGenes.Select((GeneDef x) => (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
        }
    }
    protected bool DrawSelectedGene(GeneDef gene, ref float curX, float curY, float packWidth, Rect containingRect)
    {
        bool result = false;
        if (gene == null)
        {
            return result;
        }
        Rect rect = new(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
        if (!containingRect.Overlaps(rect))
        {
            curX = rect.xMax + 14f;
            return false;
        }
        Widgets.DrawHighlight(rect);
        GUI.color = GeneCreationDialogBase.OutlineColorUnselected;
        Widgets.DrawBox(rect);
        GUI.color = Color.white;
        curX += 4f;
        GeneUIUtility.DrawBiostats(gene.biostatCpx, gene.biostatMet, gene.biostatArc, ref curX, curY, 4f);

        Rect geneRect = new(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
        GeneUIUtility.DrawGeneDef(gene, geneRect, GeneType.Xenogene, null, doBackground: false, clickable: false);
        curX += GeneCreationDialogBase.GeneSize.x + 4f;
        if (Mouse.IsOver(rect))
        {
            Widgets.DrawHighlight(rect);
        }
        if (Widgets.ButtonInvisible(rect))
        {
            result = true;
        }
        curX = Mathf.Max(curX, rect.xMax + 14f);
        return result;
    }

    protected override bool CanAccept()
    {
        if (selectedGenepack == null || selectedGene == null)
        {
            Messages.Message("OAGene_MessageNoSelectedGene".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }
        if (selectedGene.biostatArc > 0)
        {
            Messages.Message("OAGene_MessageCantSelectArcGene".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }
        return true;
    }

    protected override void OnGenesChanged()
    {
        randomChosenGroups.Clear();
        leftChosenGroups.Clear();
        cachedOverriddenGenes.Clear();
        cachedUnoverriddenGenes.Clear();
        tmpGenesWithType.Clear();
        gcx = 0;
        met = 0;
        arc = 0;
    }
    protected override void UpdateSearchResults()
    {
        quickSearchWidget.noResultsMatched = false;
        matchingGenepacks.Clear();
        matchingGenes.Clear();
        if (!quickSearchWidget.filter.Active)
        {
            return;
        }
        foreach (Genepack libraryGenepack in libraryGenepacks)
        {
            List<GeneDef> genesListForReading2 = libraryGenepack.GeneSet.GenesListForReading;
            for (int j = 0; j < genesListForReading2.Count; j++)
            {
                if (quickSearchWidget.filter.Matches(genesListForReading2[j].label))
                {
                    matchingGenepacks.Add(libraryGenepack);
                    matchingGenes.Add(genesListForReading2[j]);
                }
            }
        }
        quickSearchWidget.noResultsMatched = !matchingGenepacks.Any();
    }
}
