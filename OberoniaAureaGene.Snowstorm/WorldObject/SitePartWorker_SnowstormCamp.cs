using RimWorld;
using RimWorld.Planet;
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
            threatPoints = faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Settlement) + 100f,
            lootMarketValue = 0f
        };
        return sitePartParams;
    }
    public override string GetPostProcessedThreatLabel(Site site, SitePart sitePart)
    {
        return def.label + ": " + "KnownSiteThreatEnemyCountAppend".Translate(sitePart.expectedEnemyCount, "Enemies".Translate());
    }
}

