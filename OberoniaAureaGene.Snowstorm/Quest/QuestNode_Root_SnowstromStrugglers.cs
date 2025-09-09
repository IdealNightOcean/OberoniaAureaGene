using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.QuestGen;
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
    protected override void InitQuestParameter()
    {
        questParameter = new QuestParameter()
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
        QuestGen.slate.Set(IsMainFactionSlate, true);
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

    protected override void SetPawnsLeaveComp(string lodgerArrivalSignal, string inSignalRemovePawn)
    {
        Quest quest = QuestGen.quest;
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
        quest.AddPart(questPart_IsSnowExtremeWeather);

        //暴风雪结束时离开
        quest.SignalPassWithFaction(questParameter.faction, null, delegate
        {
            quest.Letter(letterDef: LetterDefOf.PositiveEvent, text: "[lodgersLeavingLetterText]", label: "[lodgersLeavingLetterLabel]");
        }, inSignal: outSignalNotSnowstorm);
        quest.Leave(questParameter.pawns, outSignalNotSnowstorm, sendStandardLetter: false, leaveOnCleanup: false, inSignalRemovePawn, wakeUp: true);

        DefaultDelayLeaveComp(lodgerArrivalSignal, outSignalNotSnowstorm, inSignalRemovePawn);
    }
}