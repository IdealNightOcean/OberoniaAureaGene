using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OberoniaAureaGene.Ratkin;

public class Dialog_ExtractGenes : GeneCreationDialogBase
{
    protected readonly Building_GeneExtractorBase geneExtractor;
    protected readonly Pawn extractPawn;

    protected readonly List<GeneDef> pawnGenes = [];
    protected readonly List<GeneDef> selectedGenes = [];

    protected override List<GeneDef> SelectedGenes => selectedGenes;

    public override Vector2 InitialSize => new(1016f, UI.screenHeight);
    protected override string Header => "OAGene_ExtractGenes".Translate();
    protected override string AcceptButtonLabel => "OAGene_StartExtracting".Translate();

    public Dialog_ExtractGenes(Pawn extractPawn, Building_GeneExtractorBase geneExtractor)
    {
        this.geneExtractor = geneExtractor;
        this.extractPawn = extractPawn;
        pawnGenes.AddRange(extractPawn.genes.GenesListForReading.Select(g => g.def).Where(d => d.biostatArc <= 0));
        closeOnAccept = false;
        forcePause = true;
        absorbInputAroundWindow = true;
        searchWidgetOffsetX = ButSize.x * 2f + 4f;
        pawnGenes.SortBy(g => 0f - g.displayCategory.displayPriorityInXenotype, g => g.displayCategory.label, g => g.displayOrderInCategory);
    }

    protected override void Accept()
    {
        geneExtractor.StartWork(extractPawn, selectedGenes);
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
        DrawSection(rect, selectedGenes, "SelectedGenepacks".Translate(), ref curY, ref selectedHeight, adding: false, containingRect);
        curY += 8f;
        DrawSection(rect, pawnGenes, "GenepackLibrary".Translate(), ref curY, ref unselectedHeight, adding: true, containingRect);
        if (Event.current.type == EventType.Layout)
        {
            scrollHeight = curY;
        }
        Widgets.EndScrollView();
        GUI.EndGroup();
    }

    protected void DrawSection(Rect rect, List<GeneDef> genes, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect)
    {
        float curX = 4f;
        Rect rect2 = new(10f, curY, rect.width - 16f - 10f, Text.LineHeight);
        Widgets.Label(rect2, label);
        if (!adding)
        {
            Text.Anchor = TextAnchor.UpperRight;
            GUI.color = ColoredText.SubtleGrayColor;
            Widgets.Label(rect2, "ClickToAddOrRemove".Translate());
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        curY += Text.LineHeight + 3f;
        float num = curY;
        Rect rect3 = new(0f, curY, rect.width, sectionHeight);
        Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor);
        curY += 4f;
        if (!genes.Any())
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = ColoredText.SubtleGrayColor;
            Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        else
        {
            for (int i = 0; i < genes.Count; i++)
            {
                GeneDef gene = genes[i];
                if (quickSearchWidget.filter.Active && (!matchingGenes.Contains(gene) || (adding && selectedGenes.Contains(gene))))
                {
                    continue;
                }
                float num2 = 34f + GeneSize.x + 12f;
                if (curX + num2 > rect.width - 16f)
                {
                    curX = 4f;
                    curY += GeneSize.y + 8f + 14f;
                }
                if (adding && selectedGenes.Contains(gene))
                {
                    Widgets.DrawLightHighlight(new Rect(curX, curY, num2, GeneSize.y + 8f));
                    curX += num2 + 14f;
                }
                else if (DrawGeneDef(gene, ref curX, curY, num2, containingRect))
                {
                    if (adding)
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera();
                        selectedGenes.Add(gene);
                    }
                    else
                    {
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                        selectedGenes.Remove(gene);
                    }
                    break;
                }
            }
        }
        curY += GeneSize.y + 12f;
        if (Event.current.type == EventType.Layout)
        {
            sectionHeight = curY - num;
        }
    }

    protected bool DrawGeneDef(GeneDef gene, ref float curX, float curY, float packWidth, Rect containingRect)
    {
        bool result = false;
        if (gene is null)
        {
            return result;
        }
        Rect rect = new(curX, curY, packWidth, GeneSize.y + 8f);
        if (!containingRect.Overlaps(rect))
        {
            curX = rect.xMax + 14f;
            return false;
        }
        Widgets.DrawHighlight(rect);
        GUI.color = OutlineColorUnselected;
        Widgets.DrawBox(rect);
        GUI.color = Color.white;
        curX += 4f;
        GeneUIUtility.DrawBiostats(gene.biostatCpx, gene.biostatMet, gene.biostatArc, ref curX, curY, 4f);

        Rect geneRect = new(curX, curY + 4f, GeneSize.x, GeneSize.y);
        GeneUIUtility.DrawGeneDef(gene, geneRect, GeneType.Xenogene, null, doBackground: false, clickable: false);
        curX += GeneSize.x + 4f;
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
        if (!SelectedGenes.Any())
        {
            Messages.Message("MessageNoSelectedGenepacks".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }
        return true;
    }

    protected override void UpdateSearchResults()
    {
        quickSearchWidget.noResultsMatched = false;
        matchingGenes.Clear();
        if (!quickSearchWidget.filter.Active)
        {
            return;
        }
        foreach (GeneDef gene1 in selectedGenes)
        {
            if (quickSearchWidget.filter.Matches(gene1.label))
            {
                matchingGenes.Add(gene1);
            }
        }
        foreach (GeneDef gene2 in pawnGenes)
        {
            if (selectedGenes.Contains(gene2))
            {
                continue;
            }

            if (quickSearchWidget.filter.Matches(gene2.label))
            {
                matchingGenes.Add(gene2);
            }

        }
        quickSearchWidget.noResultsMatched = !matchingGenes.Any();
    }

}
