using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class SitePartWorker_SnowstormCamp : SitePartWorker_Outpost
{
    public static IntRange EnemyCountRange = new(6, 9);
    public override void Init(Site site, SitePart sitePart)
    {
        base.Init(site, sitePart);
        site.GetComponent<SnowstormCampComp>()?.ActiveComp();
        sitePart.expectedEnemyCount = EnemyCountRange.RandomInRange;
    }

    public override SitePartParams GenerateDefaultParams(float myThreatPoints, int tile, Faction faction)
    {
        SitePartParams sitePartParams = new()
        {
            randomValue = Rand.Int,
            threatPoints = def.wantsThreatPoints ? myThreatPoints : 0f
        };
        sitePartParams.threatPoints = Mathf.Max(sitePartParams.threatPoints, FactionDefOf.OutlanderCivil.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Settlement));
        sitePartParams.lootMarketValue = ThreatPointsLootMarketValue.Evaluate(sitePartParams.threatPoints);
        return sitePartParams;
    }
}

