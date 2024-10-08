using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene.Snowstorm;

public class SitePartWorker_SnowstormRaidSource : SitePartWorker_Outpost
{
    private const int RaidMtbDays = 2;
    public override void SitePartWorkerTick(SitePart sitePart)
    {
        base.SitePartWorkerTick(sitePart);
        if (sitePart.lastRaidTick != -1 && (Find.TickManager.TicksGame < sitePart.lastRaidTick + 90000))
        {
            return;
        }
        List<Map> maps = Find.Maps;
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].IsPlayerHome && sitePart.site.IsHashIntervalTick(2500) && Rand.MTBEventOccurs(RaidMtbDays, 60000f, 2500f))
            {
                StartRaid(maps[i], sitePart);
            }
        }
    }

    private void StartRaid(Map map, SitePart sitePart)
    {
        IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
        incidentParms.forced = true;
        incidentParms.raidStrategy = Snowstrom_MiscDefOf.OAGene_SnowstormImmediateAttackBreaching;
        if (IncidentDefOf.RaidEnemy.Worker.CanFireNow(incidentParms))
        {
            IncidentDefOf.RaidEnemy.Worker.TryExecute(incidentParms);
            sitePart.lastRaidTick = Find.TickManager.TicksGame;
        }
    }

    public override void Notify_GeneratedByQuestGen(SitePart part, Slate slate, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
    {
        base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);
        int enemiesCount = GetEnemiesCount(part.site, part.parms);
        outExtraDescriptionRules.Add(new Rule_String("enemiesCount", enemiesCount.ToString()));
        outExtraDescriptionRules.Add(new Rule_String("mtbDays", (RaidMtbDays * 60000).ToStringTicksToPeriod(allowSeconds: true, shortForm: false, canUseDecimals: false)));
        outExtraDescriptionRules.Add(new Rule_String("enemiesLabel", GetEnemiesLabel(part.site, enemiesCount)));
    }

}