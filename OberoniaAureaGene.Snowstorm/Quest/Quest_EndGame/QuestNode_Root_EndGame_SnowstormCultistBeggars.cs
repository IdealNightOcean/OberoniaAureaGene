using OberoniaAurea_Frame;
using OberoniaAurea_Frame.Utility;
using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_Root_EndGame_SnowstormCultistBeggars : QuestNode
{
    private const int VisitDuration = 30000;
    private const int RequestedThingCount = 100;

    protected Faction TryResolveFaction()
    {
        Faction faction = null;
        faction ??= OAFrame_FactionUtility.ValidTempFactionsOfDef(FactionDefOf.OutlanderCivil).Where(f => !f.HostileTo(Faction.OfPlayer)).RandomElementWithFallback(null);
        faction ??= OAFrame_FactionUtility.GenerateTempFaction(FactionDefOf.OutlanderCivil);
        faction ??= Find.FactionManager.RandomNonHostileFaction(allowNonHumanlike: false);
        return faction;
    }

    protected Pawn GeneratePawn(Quest quest, Faction faction)
    {
        PawnGenerationRequest request = OAFrame_PawnGenerateUtility.CommonPawnGenerationRequest(PawnKindDefOf.Beggar, faction, forceNew: true);
        request.MustBeCapableOfViolence = true;
        request.ForceAddFreeWarmLayerIfNeeded = true;
        request.ForcedTraits = [OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor];

        Pawn pawn = quest.GeneratePawn(request);
        if (pawn.RaceProps.Humanlike)
        {
            pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_SnowstormCultist);
            Snowstorm_MiscUtility.SetColdPreparation(pawn, Snowstorm_HediffDefOf.OAGene_Hediff_ColdPreparation_Neutral);
        }
        Apparel parka = (Apparel)ThingMaker.MakeThing(ThingDefOf.Apparel_Parka, ThingDefOf.Cloth);
        pawn.apparel.Wear(parka, dropReplacedApparel: false);
        pawn.equipment.DestroyAllEquipment();
        ThingWithComps antiSnowTorch = (ThingWithComps)ThingMaker.MakeThing(Snowstorm_ThingDefOf.OAGene_AntiSnowTorch);
        pawn.equipment.AddEquipment(antiSnowTorch);
        return pawn;
    }

    protected override void RunInt()
    {
        Quest quest = QuestGen.quest;
        Slate slate = QuestGen.slate;
        Map map = Snowstorm_StoryUtility.GetHometownMap();
        float points = slate.Get("points", 0f);
        slate.Set("visitDurationTicks", VisitDuration);

        ThingDef thingDef = Snowstorm_ThingDefOf.OAGene_IceCrystal;
        slate.Set("requestedThing", thingDef);
        slate.Set("requestedThingDefName", thingDef.defName);
        slate.Set("requestedThingCount", RequestedThingCount);

        Faction faction = TryResolveFaction();
        if (faction == null)
        {
            return;
        }
        slate.Set("faction", faction);
        slate.Set("childCount", 0);
        slate.Set("allChildren", false);
        int beggarCount = Rand.RangeInclusive(1, 5);
        List<Pawn> pawns = [];
        for (int i = 0; i < beggarCount; i++)
        {
            Pawn pawn = GeneratePawn(quest, faction);
            pawns.Add(pawn);
        }
        beggarCount = pawns.Count;
        foreach (Pawn pawn in pawns)
        {
            if (pawn.inventory != null)
            {
                for (int innerCount = pawn.inventory.innerContainer.Count - 1; innerCount >= 0; innerCount--)
                {
                    if (pawn.inventory.innerContainer[innerCount].def == thingDef)
                    {
                        pawn.inventory.innerContainer[innerCount].Destroy();
                    }
                }
            }
        }

        slate.Set("beggars", pawns);
        slate.Set("beggarCount", beggarCount);
        quest.SetFactionHidden(faction);
        quest.PawnsArrive(pawns, null, map.Parent, null, joinPlayer: false, isSingleReward: false, rewardDetailsHidden: false, sendStandardLetter: false);
        string itemsReceivedSignal = QuestGen.GenerateNewSignal("ItemsReceived");
        QuestPart_BegForItems questPart_BegForItems = new()
        {
            inSignal = QuestGen.slate.Get<string>("inSignal"),
            outSignalItemsReceived = itemsReceivedSignal,
            target = pawns[0],
            faction = faction,
            mapParent = map.Parent,
            thingDef = thingDef,
            amount = RequestedThingCount
        };
        questPart_BegForItems.pawns.AddRange(pawns);
        quest.AddPart(questPart_BegForItems);

        string pawnLabelSingleOrPlural = (beggarCount > 1) ? faction.def.pawnsPlural : faction.def.pawnSingular;
        quest.Delay(VisitDuration,
            delegate
            {
                quest.Leave(pawns, null, sendStandardLetter: false, leaveOnCleanup: false);
                quest.RecordHistoryEvent(HistoryEventDefOf.CharityRefused_Beggars);
                quest.AnyColonistWithCharityPrecept(delegate
                {
                    quest.Message(string.Format("{0}: {1}", "MessageCharityEventRefused".Translate(), "MessageBeggarsLeavingWithNoItems".Translate(pawnLabelSingleOrPlural)), MessageTypeDefOf.NegativeEvent, getLookTargetsFromSignal: false, null, pawns);
                });
            },
            null, inSignalDisable: itemsReceivedSignal
        );

        string arrestedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Arrested");
        string killedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Killed");
        string leftMapSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.LeftMap");
        TaggedString leavingWithItemsMessage = (beggarCount > 1) ? "MessageBeggarsLeavingWithItemsPlural".Translate(pawnLabelSingleOrPlural) : "MessageBeggarsLeavingWithItemsSingular".Translate(pawnLabelSingleOrPlural);
        quest.AnyColonistWithCharityPrecept(
            delegate
            {
                quest.Message("MessageCharityEventFulfilled".Translate() + ": " + leavingWithItemsMessage, MessageTypeDefOf.PositiveEvent, getLookTargetsFromSignal: false, null, pawns);
                quest.RecordHistoryEvent(HistoryEventDefOf.CharityFulfilled_Beggars);
            },
            delegate
            {
                quest.Message(leavingWithItemsMessage, MessageTypeDefOf.PositiveEvent, getLookTargetsFromSignal: false, null, pawns);
            },
            inSignal: itemsReceivedSignal
        );

        quest.AnySignal(
            [killedSignal, arrestedSignal],
            delegate
            {
                quest.SignalPassActivable(
                    delegate
                    {
                        quest.AnyColonistWithCharityPrecept(delegate
                        {
                            quest.Message(string.Format("{0}: {1}", "MessageCharityEventRefused".Translate(), "MessageBeggarsLeavingWithNoItems".Translate(pawnLabelSingleOrPlural)), MessageTypeDefOf.NegativeEvent, getLookTargetsFromSignal: false, null, pawns);
                        });
                    }, null, null, null, null, itemsReceivedSignal
                );
                quest.Letter(LetterDefOf.NegativeEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, pawns, filterDeadPawnsFromLookTargets: false, "[letterTextBeggarsBetrayed]", null, "[letterLabelBeggarsBetrayed]");
                QuestPart_FactionRelationChange part = new()
                {
                    faction = faction,
                    relationKind = FactionRelationKind.Hostile,
                    canSendHostilityLetter = false,
                    inSignal = QuestGen.slate.Get<string>("inSignal")
                };
                quest.AddPart(part);
                quest.RecordHistoryEvent(HistoryEventDefOf.CharityRefused_Beggars_Betrayed);
            }
        );

        quest.SignalPassAll(delegate
        {
            QuestPart_EndGame_SatisfySnowstormCultist part = new()
            {
                inSignal = QuestGen.slate.Get<string>("inSignal")
            };
            quest.AddPart(part);
        },
        [itemsReceivedSignal, leftMapSignal]);

        quest.End(QuestEndOutcome.Fail, 0, null, QuestGenUtility.HardcodedSignalWithQuestID("faction.BecameHostileToPlayer"));

        quest.AllPawnsDespawned(pawns,
            delegate
            {
                QuestGen_End.End(quest, QuestEndOutcome.Success);
            },
            null, inSignal: leftMapSignal
        );
    }
    protected override bool TestRunInt(Slate slate)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        Map map = Snowstorm_StoryUtility.GetHometownMap();
        if (map == null)
        {
            return false;
        }
        return true;
    }
}
