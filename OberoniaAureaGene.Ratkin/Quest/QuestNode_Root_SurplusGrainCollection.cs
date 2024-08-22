using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene.Ratkin;

public class QuestNode_Root_SurplusGrainCollection : QuestNode
{
    private struct SiteSpawnCandidate
    {
        public int tile;

        public SitePartDef sitePart;
    }

    private const int SpawnRange = 9;

    private const int MinSpawnDist = 3;

    private const float MinPointsForSurpriseReinforcements = 400f;

    private const float SurpriseReinforcementChance = 0.35f;

    private static readonly SimpleCurve ExistingCampsAppearanceFrequencyMultiplier =
    [
        new CurvePoint(0f, 1f),
        new CurvePoint(2f, 1f),
        new CurvePoint(3f, 0.3f),
        new CurvePoint(4f, 0.1f)
    ];

    private const float MinPoints = 40f;

    private const string SitePartTag = "WorkSite";

    private static readonly FloatRange RewardValue = new(9000f, 12000f);

    private static bool AnySpawnCandidate(int aroundTile)
    {
        if (Find.WorldGrid[GetCandidates(aroundTile).tile].biome.campSelectionWeight > 0f)
        {
            return true;
        }
        return false;
    }

    private static Faction GetQuestFaction()
    {
        return Find.FactionManager.AllFactionsListForReading.Where(f => !f.defeated && !f.temporary && (f.def == OAGene_RatkinDefOf.Rakinia_RockRatkin || f.def == OAGene_RatkinDefOf.Rakinia_SnowRatkin)).RandomElementWithFallback();
    }

    private static Faction GetOriginalFaction(Faction questFaction)
    {
        return Find.FactionManager.AllFactionsListForReading.Where(f => !f.defeated && !f.temporary && f != questFaction && f.IsRatkinKindomFaction()).RandomElementWithFallback();
    }
    private static SiteSpawnCandidate GetCandidates(int aroundTile)
    {
        SitePartDef sitePart = OAGene_RimWorldDefOf.WorkSite_Farming;
        SitePartWorker_WorkSite worker = (SitePartWorker_WorkSite)sitePart.Worker;
        List<int> potentialTiles = PotentialSiteTiles(aroundTile);
        SiteSpawnCandidate candidate = new()
        {
            sitePart = sitePart,
            tile = potentialTiles.Where(worker.CanSpawnOn).RandomElementWithFallback(-1)
        };
        if(candidate.tile == -1)
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

        Site site = QuestGen_Sites.GenerateSite(new SitePartDefWithParams[1]
        {
            new(spawnCandidate.sitePart, new SitePartParams
            {
                threatPoints = points
            })
        }, spawnCandidate.tile, faction);
        if (site.parts.Any() && site.parts[0].lootThings.Any())
        {
            questPart_Hyperlinks.thingDefs.Add(site.parts[0].lootThings[0].ThingDef);
        }
        site.desiredThreatPoints = site.ActualThreatPoints;
        return site;
    }

    public static List<int> PotentialSiteTiles(int root)
    {
        List<int> tiles = [];
        Find.WorldFloodFiller.FloodFill(root, (int p) => !Find.World.Impassable(p) && Find.WorldGrid.ApproxDistanceInTiles(p, root) <= 9f, delegate (int p)
        {
            if (Find.WorldGrid.ApproxDistanceInTiles(p, root) >= 3f)
            {
                tiles.Add(p);
            }
        });
        return tiles;
    }

    public static float AppearanceFrequency(Map map)
    {
        float num = 1f;
        float num2 = 0f;
        List<int> list = PotentialSiteTiles(map.Tile);
        if (list.Count == 0)
        {
            return 0f;
        }
        if (!AnySpawnCandidate(map.Tile))
        {
            return 0f;
        }
        foreach (int item in list)
        {
            num2 += Find.WorldGrid[item].biome.campSelectionWeight;
        }
        num2 /= (float)list.Count;
        num *= num2;
        int num3 = 0;
        foreach (Site site in Find.WorldObjects.Sites)
        {
            if (site.MainSitePartDef.tags != null && site.MainSitePartDef.tags.Contains("WorkSite"))
            {
                num3++;
            }
        }
        num *= ExistingCampsAppearanceFrequencyMultiplier.Evaluate(num3);
        int num4 = map.mapPawns.FreeColonists.Count();
        if (num4 <= 1)
        {
            return 0f;
        }
        if (num4 == 2)
        {
            return num / 2f;
        }
        return num;
    }

    public static float BestAppearanceFrequency()
    {
        float num = 0f;
        foreach (Map map in Find.Maps)
        {
            if (map.IsPlayerHome)
            {
                num = Mathf.Max(num, AppearanceFrequency(map));
            }
        }
        return num;
    }

    protected override void RunInt()
    {
        Faction questFaction = GetQuestFaction();
        if (questFaction == null)
        {
            return;
        }
        Faction originalFation = GetOriginalFaction(questFaction);
        if (originalFation == null)
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
        slate.Set("map", map);
        Site site = GenerateSite(num, map.Tile, originalFation.def);
        Log.Message((site == null) + " " + site.Tile);
        quest.SpawnWorldObject(site);
        quest.ReserveFaction(site.Faction);
        QuestPart_InvolvedFactions questPart_InvolvedFactions = new();
        questPart_InvolvedFactions.factions.Add(questFaction);
        questPart_InvolvedFactions.factions.Add(originalFation);
        quest.AddPart(questPart_InvolvedFactions);
        int num2 = 1800000;
        quest.WorldObjectTimeout(site, num2);
        quest.Delay(num2, delegate
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
        slate.Set("timeout", num2);
        string inSignal = QuestGenUtility.HardcodedSignalWithQuestID("campSite.AllEnemiesDefeated");
        string inSignalEnabled = QuestGenUtility.HardcodedSignalWithQuestID("campSite.MapGenerated");
        string inSignal2 = QuestGenUtility.HardcodedSignalWithQuestID("campSite.MapRemoved");

        QuestPart_Choice questPart_Choice = quest.RewardChoice();
        QuestPart_Choice.Choice reward_choice = new()
        {
            rewards = { new Reward_CampLoot() }
        };
        RewardsGeneratorParams rewardsGeneratorParams = new()
        {
            rewardValue = RewardValue.RandomInRange,
            thingRewardItemsOnly = true,
            giverFaction = questFaction
        };
        reward_choice.rewards.AddRange(RewardsGenerator.Generate(rewardsGeneratorParams));
        questPart_Choice.choices.Add(reward_choice);
        if (num >= 400f)
        {
            quest.SurpriseReinforcements(inSignalEnabled, site, site.Faction, 0.35f);
        }
        quest.Notify_PlayerRaidedSomeone(null, site, inSignal);
        quest.FactionGoodwillChange(originalFation, -60, inSignal: inSignalEnabled, historyEvent: HistoryEventDefOf.AttackedSettlement);
        quest.End(QuestEndOutcome.Success, 0, null, inSignal2);
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
        Map map = QuestGen_Get.GetMap();
        return map != null;
    }
}
