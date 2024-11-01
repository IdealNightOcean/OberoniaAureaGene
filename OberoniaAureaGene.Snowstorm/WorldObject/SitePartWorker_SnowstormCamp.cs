using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class SitePartWorker_SnowstormCamp : SitePartWorker_Outpost
{
    public override void Init(Site site, SitePart sitePart)
    {
        base.Init(site, sitePart);
        site.GetComponent<SnowstormCampComp>()?.ActiveComp();
    }

    public override SitePartParams GenerateDefaultParams(float myThreatPoints, int tile, Faction faction)
    {
        SitePartParams sitePartParams = new()
        {
            randomValue = Rand.Int,
            threatPoints = def.wantsThreatPoints ? myThreatPoints : 0f
        }; ;
        sitePartParams.threatPoints = Mathf.Max(sitePartParams.threatPoints, FactionDefOf.OutlanderCivil.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Settlement));
        sitePartParams.lootMarketValue = ThreatPointsLootMarketValue.Evaluate(sitePartParams.threatPoints);
        return sitePartParams;
    }
}

