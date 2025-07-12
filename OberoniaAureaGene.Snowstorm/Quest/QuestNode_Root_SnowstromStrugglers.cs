using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_Root_SnowstormStrugglers : QuestNode_Root_RefugeeBase
{
    private static readonly IntRange FoodCount = new(5, 7);
    protected override bool TestRunInt(Slate slate)
    {
        Map map = QuestGen_Get.GetMap();
        if (map is null)
        {
            return false;
        }
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        return Find.FactionManager.RandomAlliedFaction(allowNonHumanlike: false) is not null;
    }
    protected override QuestParameter InitQuestParameter(Faction faction)
    {
        return new QuestParameter(faction, QuestGen_Get.GetMap())
        {
            allowAssaultColony = false,
            allowLeave = false,
            allowBadThought = false,
            LodgerCount = Rand.RangeInclusive(2, 4),
            ChildCount = 0,
            questDurationTicks = Rand.RangeInclusive(5, 10) * 60000,
            goodwillSuccess = 25,
            goodwillFailure = -25
        };
    }

    protected override Faction GetOrGenerateFaction()
    {
        return Find.FactionManager.RandomAlliedFaction(allowNonHumanlike: false);
    }

    protected override void PostPawnGenerated(Pawn pawn)
    {
        pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_HopeForSurvival);
        pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_SnowstormStrugglers);
        Thing food = ThingMaker.MakeThing(ThingDefOf.MealSimple);
        food.stackCount = FoodCount.RandomInRange;
        pawn.inventory.innerContainer.TryAdd(food);
    }

    protected override void SetPawnsLeaveComp(QuestParameter questParameter, List<Pawn> pawns, string inSignalEnable, string inSignalRemovePawn)
    {
        Quest quest = questParameter.quest;
        Faction faction = questParameter.faction;

        string outSignalNotSnowstorm = QuestGenUtility.HardcodedSignalWithQuestID("snowstormEnd");
        QuestPart_IsSnowExtremeWeather questPart_IsSnowExtremeWeather = new()
        {
            inSignalEnable = QuestGen.slate.Get<string>("inSignal"),
            outSignalNotSnowstorm = outSignalNotSnowstorm,
            snowstormOutSignal = false,
            notSnowstormOutSignal = true,
            checkInterval = 2500,
            map = questParameter.map,
        };
        questParameter.quest.AddPart(questPart_IsSnowExtremeWeather);

        //暴风雪结束时离开
        quest.SignalPassWithFaction(faction, null, delegate
        {
            quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgersLeavingLetterText]", null, "[lodgersLeavingLetterLabel]");
        }, inSignal: outSignalNotSnowstorm);
        quest.Leave(pawns, outSignalNotSnowstorm, sendStandardLetter: false, leaveOnCleanup: false, inSignalRemovePawn, wakeUp: true);

        base.SetPawnsLeaveComp(questParameter, pawns, inSignalEnable, inSignalRemovePawn);
    }
}