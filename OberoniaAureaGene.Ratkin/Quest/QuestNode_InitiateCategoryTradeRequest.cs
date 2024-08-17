﻿using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class QuestNode_InitiateCategoryTradeRequest : QuestNode
{
    public SlateRef<ThingCategoryDef> requestedCategoryDef;
    public SlateRef<int> requestedThingCount;
    public SlateRef<int> requestQuality = -1;
    public SlateRef<Settlement> settlement;
    public SlateRef<int> duration;
    public SlateRef<bool> isApparel = false;

    protected override bool TestRunInt(Slate slate)
    {
        return settlement.GetValue(slate) != null && requestedThingCount.GetValue(slate) > 0 && requestedCategoryDef.GetValue(slate) != null && duration.GetValue(slate) > 0;
    }
    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        QuestPart_InitiateCategoryTradeRequest questPart_InitiateCategoryTradeRequest = new()
        {
            settlement = settlement.GetValue(slate),
            requestedCategoryDef = requestedCategoryDef.GetValue(slate),
            requestedCount = requestedThingCount.GetValue(slate),
            requestDuration = duration.GetValue(slate),
            requestQuality = requestQuality.GetValue(slate),
            isApparel = isApparel.GetValue(slate),
            inSignal = slate.Get<string>("inSignal")
        };
        QuestGen.quest.AddPart(questPart_InitiateCategoryTradeRequest);
    }

}
