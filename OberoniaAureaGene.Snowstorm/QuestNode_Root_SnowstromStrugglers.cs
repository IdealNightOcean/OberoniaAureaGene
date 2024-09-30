using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_Root_SnowstromStrugglers : QuestNode_Root_RefugeeBase
{
    protected override bool TestRunInt(Slate slate)
    {
        Map map = QuestGen_Get.GetMap();
        if (map == null)
        {
            return false;
        }
        return SnowstormUtility.IsSnowExtremeWeather(map);
    }
    protected override Faction GetOrGenerateFaction()
    {
        return Find.FactionManager.RandomAlliedFaction(allowNonHumanlike: false);
    }
    protected override void RunInt()
    {
        Quest quest = QuestGen.quest;
        Slate slate = QuestGen.slate;
        Map map = QuestGen_Get.GetMap();
        Faction faction = GetOrGenerateFaction();
        if (map == null || faction == null)
        {
            return;
        }
        int questDurationDays = QuestDurationDaysRange.RandomInRange;
        int questDurationTicks = questDurationDays * 60000;

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
        slate.Set("lodgers", pawns);
        Pawn asker = pawns.First();
        QuestPart_ExtraFaction questPart_ExtraFaction = quest.ExtraFaction(faction, pawns, ExtraFactionType.MiniFaction, areHelpers: false, [lodgerRecruitedSignal, lodgerBecameMutantSignal]);
        quest.PawnsArrive(pawns, null, map.Parent, null, joinPlayer: true, null, "[lodgersArriveLetterLabel]", "[lodgersArriveLetterText]");
        quest.SetAllApparelLocked(pawns);

        string assaultColonySignal = QuestGen.GenerateNewSignal("AssaultColony");
        void AssaultColony()
        {
            int num4 = Mathf.FloorToInt(MutinyTimeRange.RandomInRange * (float)questDurationTicks);
            quest.Delay(num4, delegate
            {
                quest.Letter(LetterDefOf.ThreatBig, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[mutinyLetterText]", null, "[mutinyLetterLabel]");
                quest.SignalPass(null, null, assaultColonySignal);
                QuestGen_End.End(quest, QuestEndOutcome.Unknown);
            }, null, null, null, reactivatable: false, null, null, isQuestTimeout: false, null, null, "Mutiny (" + num4.ToStringTicksToDays() + ")");
        }
        void BetrayColony()
        {
            int num3 = Mathf.FloorToInt(BetrayalOfferTimeRange.RandomInRange * (float)questDurationTicks);
            Pawn factionOpponent = quest.GetPawn(FactionOpponentPawnParams);
            quest.Delay(num3, delegate
            {
                QuestPart_AddQuest_RefugeeBetrayal part = new()
                {
                    acceptee = quest.AccepterPawn,
                    lodgers = pawns,
                    refugeeFaction = questPart_ExtraFaction.extraFaction,
                    factionOpponent = factionOpponent,
                    inSignal = QuestGen.slate.Get<string>("inSignal"),
                    inSignalRemovePawn = lodgerArrestedOrRecruited,
                    parent = quest,
                    asker = asker,
                    mapParent = map.Parent,
                    sendAvailableLetter = true
                };
                quest.AddPart(part);
            }, null, null, null, reactivatable: false, null, null, isQuestTimeout: false, null, null, "BetrayalOffer (" + num3.ToStringTicksToDays() + ")");
        }
        if (Find.Storyteller.difficulty.allowViolentQuests)
        {
            List<Tuple<float, Action>> list2 =
            [
                !Find.Storyteller.difficulty.ChildRaidersAllowed &&  childCount> 0 ? Tuple.Create<float, Action>(0.25f, delegate
                {
                }) : Tuple.Create(0.25f, AssaultColony),
            ];
            if (QuestGen_Pawns.GetPawnTest(FactionOpponentPawnParams, out var _))
            {
                list2.Add(Tuple.Create(0.25f, BetrayColony));
            }
            list2.Add(Tuple.Create<float, Action>(0.5f, delegate
            {
            }));
            if (list2.TryRandomElementByWeight((Tuple<float, Action> t) => t.Item1, out var result2))
            {
                result2.Item2();
            }

            QuestPart_OARefugeeInteractions questPart_StrugglersInteractions = SnowstromStrugglersInteractions(faction, map.Parent, slate);
            questPart_StrugglersInteractions.inSignalArrested = lodgerArrestedSignal;
            questPart_StrugglersInteractions.inSignalRecruited = lodgerRecruitedSignal;
            questPart_StrugglersInteractions.inSignalAssaultColony = assaultColonySignal;
            questPart_StrugglersInteractions.pawns.AddRange(pawns);
            quest.AddPart(questPart_StrugglersInteractions);

            quest.Delay(questDurationTicks, delegate
            {
                quest.SignalPassWithFaction(faction, null, delegate
                {
                    quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgersLeavingLetterText]", null, "[lodgersLeavingLetterLabel]");
                });
                quest.Leave(pawns, null, sendStandardLetter: false, leaveOnCleanup: false, lodgerArrestedOrRecruited, wakeUp: true);
            }, null, null, null, reactivatable: false, null, null, isQuestTimeout: false, "GuestsDepartsIn".Translate(), "GuestsDepartsOn".Translate(), "QuestDelay");

            SetQuestEndComp(quest, questPart_StrugglersInteractions, pawns, faction);

            quest.SignalPass(delegate
            {
                if (Rand.Chance(0.5f))
                {
                    float num2 = (float)(lodgerCount * questDurationDays) * 55f;
                    FloatRange marketValueRange = new FloatRange(0.7f, 1.3f) * num2 * Find.Storyteller.difficulty.EffectiveQuestRewardValueFactor;
                    quest.AddQuestRefugeeDelayedReward(quest.AccepterPawn, faction, pawns, marketValueRange);
                }
                quest.End(QuestEndOutcome.Success, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
            }, questPart_StrugglersInteractions.outSignalLast_LeftMapAllHealthy);
            slate.Set("lodgerCount", lodgerCount);
            slate.Set("lodgersCountMinusOne", lodgerCount - 1);
            slate.Set("childCount", childCount);
            slate.Set("allButOneChildren", childCount == lodgerCount - 1);
            slate.Set("asker", asker);
            slate.Set("map", map);
            slate.Set("questDurationTicks", questDurationTicks);
            slate.Set("faction", faction);
        }
    }
    private void SetAward(Quest quest)
    {

        QuestPart_Choice questPart_Choice = quest.RewardChoice();
        QuestPart_Choice.Choice choice = new()
        {
            rewards =
            {
                new Reward_VisitorsHelp(),
                new Reward_PossibleFutureReward()
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

        inSignalEnable = slate.Get<string>("inSignal"),
        inSignalSurgeryViolation = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.SurgeryViolation"),
        inSignalPsychicRitualTarget = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.PsychicRitualTarget"),
        inSignalDestroyed = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.Destroyed"),
        inSignalKidnapped = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.Kidnapped"),
        inSignalLeftMap = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.LeftMap"),
        inSignalBanished = QuestGenUtility.HardcodedSignalWithQuestID("lodgers.Banished"),
        outSignalDestroyed_AssaultColony = QuestGen.GenerateNewSignal("LodgerDestroyed_AssaultColony"),
        outSignalDestroyed_LeaveColony = QuestGen.GenerateNewSignal("LodgerDestroyed_LeaveColony"),
        outSignalDestroyed_BadThought = QuestGen.GenerateNewSignal("LodgerDestroyed_BadThought"),
        outSignalArrested_AssaultColony = QuestGen.GenerateNewSignal("LodgerArrested_AssaultColony"),
        outSignalArrested_LeaveColony = QuestGen.GenerateNewSignal("LodgerArrested_LeaveColony"),
        outSignalArrested_BadThought = QuestGen.GenerateNewSignal("LodgerArrested_BadThought"),
        outSignalSurgeryViolation_AssaultColony = QuestGen.GenerateNewSignal("LodgerSurgeryViolation_AssaultColony"),
        outSignalSurgeryViolation_LeaveColony = QuestGen.GenerateNewSignal("LodgerSurgeryViolation_LeaveColony"),
        outSignalSurgeryViolation_BadThought = QuestGen.GenerateNewSignal("LodgerSurgeryViolation_BadThought"),
        outSignalPsychicRitualTarget_AssaultColony = QuestGen.GenerateNewSignal("LodgerPsychicRitualTarget_AssaultColony"),
        outSignalPsychicRitualTarget_LeaveColony = QuestGen.GenerateNewSignal("LodgerPsychicRitualTarget_LeaveColony"),
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

        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalDestroyed_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerDiedMemoryThoughtLetterText]", null, "[lodgerDiedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalDestroyed_AssaultColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerDiedAttackPlayerLetterText]", null, "[lodgerDiedAttackPlayerLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalDestroyed_LeaveColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerDiedLeaveMapLetterText]", null, "[lodgerDiedLeaveMapLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalLast_Destroyed, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgersAllDiedLetterText]", null, "[lodgersAllDiedLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalArrested_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerArrestedMemoryThoughtLetterText]", null, "[lodgerArrestedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalArrested_AssaultColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerArrestedAttackPlayerLetterText]", null, "[lodgerArrestedAttackPlayerLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalArrested_LeaveColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerArrestedLeaveMapLetterText]", null, "[lodgerArrestedLeaveMapLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalLast_Arrested, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgersAllArrestedLetterText]", null, "[lodgersAllArrestedLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalSurgeryViolation_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerViolatedMemoryThoughtLetterText]", null, "[lodgerViolatedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalSurgeryViolation_AssaultColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerViolatedAttackPlayerLetterText]", null, "[lodgerViolatedAttackPlayerLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalSurgeryViolation_LeaveColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerViolatedLeaveMapLetterText]", null, "[lodgerViolatedLeaveMapLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalPsychicRitualTarget_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerPsychicRitualTargetMemoryThoughtLetterText]", null, "[lodgerPsychicRitualTargetMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalPsychicRitualTarget_AssaultColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerPsychicRitualTargetAttackPlayerLetterText]", null, "[lodgerPsychicRitualTargetAttackPlayerLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent, questPart_StrugglersInteractions.outSignalPsychicRitualTarget_LeaveColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[lodgerPsychicRitualTargetLeaveMapLetterText]", null, "[lodgerPsychicRitualTargetLeaveMapLetterLabel]");
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerDied, questPart_StrugglersInteractions.outSignalDestroyed_BadThought);
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerArrested, questPart_StrugglersInteractions.outSignalArrested_BadThought);
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerSurgicallyViolated, questPart_StrugglersInteractions.outSignalSurgeryViolation_BadThought);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalDestroyed_AssaultColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalDestroyed_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalLast_Destroyed);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalArrested_AssaultColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalArrested_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalLast_Arrested);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalSurgeryViolation_AssaultColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalSurgeryViolation_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalPsychicRitualTarget_AssaultColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalPsychicRitualTarget_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalLast_Kidnapped, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_StrugglersInteractions.outSignalLast_Banished, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Success, 0, null, questPart_StrugglersInteractions.outSignalLast_Recruited, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
        quest.End(QuestEndOutcome.Success, 0, null, questPart_StrugglersInteractions.outSignalLast_LeftMapAllNotHealthy, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
    }
}