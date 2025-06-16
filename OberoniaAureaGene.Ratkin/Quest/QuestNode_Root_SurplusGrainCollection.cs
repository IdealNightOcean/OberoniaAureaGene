using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene.Ratkin;

public class QuestNode_Root_SurplusGrainCollection : QuestNode
{
    private struct SiteSpawnCandidate
    {
        public PlanetTile tile;

        public SitePartDef sitePart;
    }

    private static readonly FloatRange RewardValue = new(9000f, 12000f);

    private static Faction GetQuestFaction()
    {
        return Find.FactionManager.AllFactionsListForReading.Where(f => !f.defeated && !f.temporary && (f.def == OAGene_RatkinDefOf.Rakinia_RockRatkin || f.def == OAGene_RatkinDefOf.Rakinia_SnowRatkin)).RandomElementWithFallback();
    }

    private static Faction GetOriginalFaction(Faction questFaction)
    {
        return Find.FactionManager.AllFactionsListForReading.Where(f => !f.defeated && !f.temporary && f != questFaction && f.IsRatkinKindomFaction()).RandomElementWithFallback();
    }
    private static SiteSpawnCandidate GetCandidates(PlanetTile aroundTile)
    {
        SitePartDef sitePart = OAGene_RimWorldDefOf.WorkSite_Farming;
        SitePartWorker_WorkSite worker = (SitePartWorker_WorkSite)sitePart.Worker;
        List<PlanetTile> potentialTiles = PotentialSiteTiles(aroundTile);
        SiteSpawnCandidate candidate = new()
        {
            sitePart = sitePart,
            tile = potentialTiles.Where(worker.CanSpawnOn).RandomElementWithFallback(PlanetTile.Invalid)
        };
        if (!candidate.tile.Valid)
        {
            candidate.tile = potentialTiles.RandomElement();
        }
        return candidate;
    }

    public static Site GenerateSite(float points, int aroundTile, FactionDef originalFactionDef)
    {
        SiteSpawnCandidate spawnCandidate = GetCandidates(aroundTile);
        SitePartWorker_WorkSite sitePartWorker = (SitePartWorker_WorkSite)spawnCandidate.sitePart.Worker;
        QuestPart_Hyperlinks questPart_Hyperlinks = new();
        QuestGen.quest.AddPart(questPart_Hyperlinks);
        FactionGeneratorParms parms = new(originalFactionDef, default, true);
        parms.ideoGenerationParms = new IdeoGenerationParms(parms.factionDef, forceNoExpansionIdeo: false, sitePartWorker.DisallowedPrecepts.ToList());
        List<FactionRelation> list = [];
        foreach (Faction faction1 in Find.FactionManager.AllFactionsListForReading)
        {
            if (!faction1.def.PermanentlyHostileTo(parms.factionDef))
            {
                if (faction1 == Faction.OfPlayer)
                {
                    list.Add(new FactionRelation
                    {
                        other = faction1,
                        kind = FactionRelationKind.Hostile
                    });
                }
                else
                {
                    list.Add(new FactionRelation
                    {
                        other = faction1,
                        kind = FactionRelationKind.Neutral
                    });
                }
            }
        }
        Faction faction = FactionGenerator.NewGeneratedFactionWithRelations(parms, list);
        faction.temporary = true;
        Find.FactionManager.Add(faction);

        Site site = QuestGen_Sites.GenerateSite(
        [
            new(spawnCandidate.sitePart, new SitePartParams
            {
                threatPoints = points
            })
        ], spawnCandidate.tile, faction);
        if (site.parts.Any() && site.parts[0].lootThings.Any())
        {
            questPart_Hyperlinks.thingDefs.Add(site.parts[0].lootThings[0].ThingDef);
        }
        site.desiredThreatPoints = site.ActualThreatPoints;
        return site;
    }

    public static List<PlanetTile> PotentialSiteTiles(PlanetTile root)
    {
        List<PlanetTile> tiles = [];
        root.Layer.Filler.FloodFill(root, (PlanetTile p) => !Find.World.Impassable(p) && Find.WorldGrid.ApproxDistanceInTiles(p, root) <= 9f, delegate (PlanetTile p)
        {
            if (Find.WorldGrid.ApproxDistanceInTiles(p, root) >= 3f)
            {
                tiles.Add(p);
            }
        });
        return tiles;
    }

    protected override void RunInt()
    {
        Faction questFaction = GetQuestFaction();
        if (questFaction is null)
        {
            return;
        }
        Faction originalFation = GetOriginalFaction(questFaction);
        if (originalFation is null)
        {
            return;
        }
        Quest quest = QuestGen.quest;
        Slate slate = QuestGen.slate;
        QuestGenUtility.RunAdjustPointsForDistantFight();
        float num = slate.Get("points", 0f);
        if (num < 40f)
        {
            num = 40f;
        }
        Map map = QuestGen_Get.GetMap();
        Pawn asker = questFaction.leader;
        slate.Set("map", map);
        slate.Set("asker", asker);
        Site site = GenerateSite(num, map.Tile, originalFation.def);
        quest.SpawnWorldObject(site);
        quest.ReserveFaction(site.Faction);
        QuestPart_InvolvedFactions questPart_InvolvedFactions = new();
        questPart_InvolvedFactions.factions.Add(questFaction);
        questPart_InvolvedFactions.factions.Add(originalFation);
        quest.AddPart(questPart_InvolvedFactions);
        int timeout = 1800000;
        quest.WorldObjectTimeout(site, timeout);
        quest.Delay(timeout, delegate
        {
            QuestGen_End.End(quest, QuestEndOutcome.Fail);
        });
        quest.Message("MessageCampDetected".Translate(site.Named("CAMP"), site.Faction.Named("FACTION")), MessageTypeDefOf.NeutralEvent, getLookTargetsFromSignal: false, null, new LookTargets(site));
        SitePart sitePart = site.parts[0];
        if (!sitePart.things.NullOrEmpty())
        {
            ThingDef def = sitePart.things.First().def;
            int num3 = 0;
            foreach (Thing item2 in sitePart.things)
            {
                if (item2.def == def)
                {
                    num3 += item2.stackCount;
                }
            }
            QuestGen.AddQuestDescriptionRules(
            [
                new Rule_String("loot", def.label + " x" + num3)
            ]);
        }
        slate.Set("campSite", site);
        slate.Set("questFaction", questFaction);
        slate.Set("originalFation", originalFation);
        slate.Set("faction", site.Faction);
        slate.Set("timeout", timeout);
        string inSignalSuccess = QuestGenUtility.HardcodedSignalWithQuestID("campSite.AllEnemiesDefeated");
        string inSignalEnabled = QuestGenUtility.HardcodedSignalWithQuestID("campSite.MapGenerated");
        string inSignalEnd = QuestGenUtility.HardcodedSignalWithQuestID("campSite.MapRemoved");

        if (num >= 400f)
        {
            quest.SurpriseReinforcements(inSignalEnabled, site, site.Faction, 0.35f);
        }
        quest.Notify_PlayerRaidedSomeone(null, site, inSignalSuccess);
        quest.FactionGoodwillChange(originalFation, -60, inSignal: inSignalEnabled, historyEvent: HistoryEventDefOf.AttackedSettlement);
        //任务奖励

        quest.GiveRewards(new RewardsGeneratorParams
        {
            rewardValue = RewardValue.RandomInRange,
            thingRewardItemsOnly = true,
            giverFaction = questFaction
        }, inSignalSuccess, addCampLootReward: true, asker: asker);

        quest.End(QuestEndOutcome.Success, 0, null, inSignalEnd);
        QuestGen.AddQuestDescriptionRules(
        [
            new Rule_String("siteLabel", site.Label)
        ]);
    }

    protected override bool TestRunInt(Slate slate)
    {
        if (!Find.Storyteller.difficulty.allowViolentQuests)
        {
            return false;
        }
        if (WealthUtility.PlayerWealth < 80000f)
        {
            return false;
        }
        Map map = QuestGen_Get.GetMap();
        return map is not null;
    }
}
