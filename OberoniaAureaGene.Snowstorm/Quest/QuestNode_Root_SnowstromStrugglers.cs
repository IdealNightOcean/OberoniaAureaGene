using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_Root_SnowstromStrugglers : QuestNode_Root_RefugeeBase
{
    protected override IntRange LodgerCount => new(2, 4);
    private static readonly IntRange FoodCount = new(5, 7);
    protected override bool TestRunInt(Slate slate)
    {
        Map map = QuestGen_Get.GetMap();
        if (map == null)
        {
            return false;
        }
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        return Find.FactionManager.RandomAlliedFaction(allowNonHumanlike: false) != null;
    }
    protected override Faction GetOrGenerateFaction()
    {
        return Find.FactionManager.RandomAlliedFaction(allowNonHumanlike: false);
    }
    protected override List<Pawn> GeneratePawns(int lodgerCount, int childCount, Faction faction, Map map, Quest quest, string lodgerRecruitedSignal = null)
    {
        List<Pawn> pawns = base.GeneratePawns(lodgerCount, childCount, faction, map, quest, lodgerRecruitedSignal);
        if (pawns != null)
        {
            foreach (Pawn p in pawns)
            {
                p.health.AddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_HopeForSurvival);
                p.health.AddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_SnowstromStrugglers);
                Thing food = ThingMaker.MakeThing(ThingDefOf.MealSimple);
                food.stackCount = FoodCount.RandomInRange;
                p.inventory.innerContainer.TryAdd(food);
            }
        }
        return pawns;
    }
    protected override void RunInt()
    {
        Quest quest = QuestGen.quest;
        Slate slate = QuestGen.slate;
        Map map = QuestGen_Get.GetMap();
        if (map == null || !SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return;
        }
        Faction faction = GetOrGenerateFaction();
        if (faction == null)
        {
            return;
        }

        string lodgerRecruitedSignal = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.Recruited");
        string lodgerArrestedSignal = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.Arrested");
        string lodgerBecameMutantSignal = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.BecameMutant");

        string lodgerArrestedOrRecruited = QuestGen.GenerateNewSignal("Lodger_ArrestedOrRecruited");
        quest.AnySignal([lodgerRecruitedSignal, lodgerArrestedSignal], null, [lodgerArrestedOrRecruited]);

        int lodgerCount = LodgerCount.RandomInRange;
        int childCount = 0;
        if (Find.Storyteller.difficulty.ChildrenAllowed && lodgerCount >= 2)
        {
            new List<(int, float)>
            {
                (0, 0.7f),
                (Rand.Range(1, lodgerCount / 2), 0.2f),
                (lodgerCount - 1, 0.1f)
            }.TryRandomElementByWeight(((int, float) p) => p.Item2, out (int, float) result);
            childCount = result.Item1;
            slate.Set("childCount", childCount);
            if (childCount == lodgerCount - 1)
            {
                slate.Set("allButOneChildren", true);
            }
        }
        List<Pawn> pawns = GeneratePawns(lodgerCount, childCount, faction, map, quest, lodgerRecruitedSignal);
        Pawn asker = pawns.First();
        slate.Set("lodgers", pawns);
        slate.Set("asker", asker);

        QuestPart_ExtraFaction questPart_ExtraFaction = quest.ExtraFaction(faction, pawns, ExtraFactionType.MiniFaction, areHelpers: false, [lodgerRecruitedSignal, lodgerBecameMutantSignal]);
        quest.PawnsArrive(pawns, null, map.Parent, null, joinPlayer: true, null, "[lodgersArriveLetterLabel]", "[lodgersArriveLetterText]");
        quest.SetAllApparelLocked(pawns);

        SetAward(faction, quest);

        QuestPart_OARefugeeInteractions questPart_StrugglersInteractions = SnowstromStrugglersInteractions(faction, map.Parent, slate);
        questPart_StrugglersInteractions.inSignalArrested = lodgerArrestedSignal;
        questPart_StrugglersInteractions.inSignalRecruited = lodgerRecruitedSignal;
        questPart_StrugglersInteractions.pawns.AddRange(pawns);
        quest.AddPart(questPart_StrugglersInteractions);

        string outSignalNotSnowstorm = QuestGenUtility.HardcodedSignalWithQuestID("snowstormEnd");
        QuestPart_IsSnowExtremeWeather questPart_IsSnowExtremeWeather = new()
        {
            inSignalEnable = QuestGen.slate.Get<string>("inSignal"),
            outSignalNotSnowstorm = outSignalNotSnowstorm,
            snowstormOutSignal = false,
            notSnowstormOutSignal = true,
            checkInterval = 2500,
            map = map,
        };
        quest.AddPart(questPart_IsSnowExtremeWeather);

        //暴风雪结束时离开
        quest.SignalPassWithFaction(faction, null, delegate
        {
            quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgersLeavingLetterText]", null, "[lodgersLeavingLetterLabel]");
        }, inSignal: outSignalNotSnowstorm);
        quest.Leave(pawns, outSignalNotSnowstorm, sendStandardLetter: false, leaveOnCleanup: false, lodgerArrestedOrRecruited, wakeUp: true);

        SetQuestEndComp(quest, questPart_StrugglersInteractions, pawns, faction);
        quest.SignalPassAny(delegate
        {
            if (Rand.Chance(0.5f))
            {
                float num2 = lodgerCount * QuestDurationDaysRange.RandomInRange * 55f;
                FloatRange marketValueRange = new FloatRange(0.7f, 1.3f) * num2 * Find.Storyteller.difficulty.EffectiveQuestRewardValueFactor;
                quest.AddQuestRefugeeDelayedReward(quest.AccepterPawn, faction, pawns, marketValueRange);
            }
            quest.End(QuestEndOutcome.Success, 25, faction, null, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        }, [questPart_StrugglersInteractions.outSignalLast_LeftMapAllHealthy, questPart_StrugglersInteractions.outSignalLast_LeftMapAllNotHealthy]);

        slate.Set("map", map);
        slate.Set("faction", faction);
        slate.Set("lodgerCount", lodgerCount);
        slate.Set("lodgersCountMinusOne", lodgerCount - 1);
        slate.Set("childCount", childCount);

    }
    private void SetAward(Faction faction, Quest quest)
    {
        QuestPart_Choice questPart_Choice = quest.RewardChoice();
        QuestPart_Choice.Choice choice = new()
        {
            rewards =
            {
                new Reward_VisitorsHelp(),
                new Reward_PossibleFutureReward(),
                new Reward_Goodwill()
                {
                    amount = 25,
                    faction = faction
                },
            }
        };
        if (ModsConfig.IdeologyActive && Faction.OfPlayer.ideos.FluidIdeo != null)
        {
            choice.rewards.Add(new Reward_DevelopmentPoints(quest));
        }
        questPart_Choice.choices.Add(choice);

    }
    private QuestPart_OARefugeeInteractions SnowstromStrugglersInteractions(Faction faction, MapParent mapParent, Slate slate) => new()
    {
        allowAssaultColony = false,
        allowLeave = false,
        allowBadThought = true,
        inSignalEnable = slate.Get<string>("inSignal"),

        inSignalSurgeryViolation = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.SurgeryViolation"),
        inSignalPsychicRitualTarget = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.PsychicRitualTarget"),
        inSignalDestroyed = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.Destroyed"),
        inSignalKidnapped = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.Kidnapped"),
        inSignalLeftMap = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.LeftMap"),
        inSignalBanished = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.Banished"),
        outSignalDestroyed_BadThought = QuestGen.GenerateNewSignal("LodgerDestroyed_BadThought"),
        outSignalArrested_BadThought = QuestGen.GenerateNewSignal("LodgerArrested_BadThought"),
        outSignalSurgeryViolation_BadThought = QuestGen.GenerateNewSignal("LodgerSurgeryViolation_BadThought"),
        outSignalPsychicRitualTarget_BadThought = QuestGen.GenerateNewSignal("LodgerPsychicRitualTarget_BadThought"),
        outSignalLast_Destroyed = QuestGen.GenerateNewSignal("LastLodger_Destroyed"),
        outSignalLast_Arrested = QuestGen.GenerateNewSignal("LastLodger_Arrested"),
        outSignalLast_Kidnapped = QuestGen.GenerateNewSignal("LastLodger_Kidnapped"),
        outSignalLast_Recruited = QuestGen.GenerateNewSignal("LastLodger_Recruited"),
        outSignalLast_LeftMapAllHealthy = QuestGen.GenerateNewSignal("LastLodger_LeftMapAllHealthy"),
        outSignalLast_LeftMapAllNotHealthy = QuestGen.GenerateNewSignal("LastLodger_LeftMapAllNotHealthy"),
        outSignalLast_Banished = QuestGen.GenerateNewSignal("LastLodger_Banished"),

        faction = faction,
        mapParent = mapParent,
        signalListenMode = QuestPart.SignalListenMode.Always
    };
    private void SetQuestEndComp(Quest quest, QuestPart_OARefugeeInteractions questPart_StrugglersInteractions, List<Pawn> pawns, Faction faction)
    {
        int failGoodwill = -25;

        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalDestroyed_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerDiedMemoryThoughtLetterText]", null, "[lodgerDiedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalLast_Destroyed, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgersAllDiedLetterText]", null, "[lodgersAllDiedLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalArrested_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerArrestedMemoryThoughtLetterText]", null, "[lodgerArrestedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalLast_Arrested, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgersAllArrestedLetterText]", null, "[lodgersAllArrestedLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalSurgeryViolation_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerViolatedMemoryThoughtLetterText]", null, "[lodgerViolatedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalPsychicRitualTarget_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerPsychicRitualTargetMemoryThoughtLetterText]", null, "[lodgerPsychicRitualTargetMemoryThoughtLetterLabel]");
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerDied, questPart_StrugglersInteractions.outSignalDestroyed_BadThought);
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerArrested, questPart_StrugglersInteractions.outSignalArrested_BadThought);
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerSurgicallyViolated, questPart_StrugglersInteractions.outSignalSurgeryViolation_BadThought);
        quest.End(QuestEndOutcome.Fail, failGoodwill, faction, questPart_StrugglersInteractions.outSignalLast_Destroyed);
        quest.End(QuestEndOutcome.Fail, failGoodwill, faction, questPart_StrugglersInteractions.outSignalLast_Arrested);
        quest.End(QuestEndOutcome.Fail, failGoodwill, faction, questPart_StrugglersInteractions.outSignalLast_Kidnapped, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, failGoodwill, faction, questPart_StrugglersInteractions.outSignalLast_Banished, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Success, 25, faction, questPart_StrugglersInteractions.outSignalLast_Recruited, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
    }
}