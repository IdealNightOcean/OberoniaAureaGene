using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class SymbolResolver_Hometown : SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        Map map = BaseGen.globalSettings.map;
        Faction faction = rp.faction ?? Faction.OfPlayer;
        int edgeDefenseWidth = 0;
        if (rp.edgeDefenseWidth.HasValue)
        {
            edgeDefenseWidth = rp.edgeDefenseWidth.Value;
        }
        else if (rp.rect.Width >= 20 && rp.rect.Height >= 20 && ((int)faction.def.techLevel >= 4 || Rand.Bool))
        {
            edgeDefenseWidth = Rand.Bool ? 2 : 4;
        }
        float minEmptyNodes = rp.rect.Area / 144f * 0.17f;
        BaseGen.globalSettings.minEmptyNodes = (minEmptyNodes >= 1f) ? GenMath.RoundRandom(minEmptyNodes) : 0;

        // BaseGen.symbolStack.Push("outdoorLighting", rp);

        ResolveParams resolveParams1 = rp;
        resolveParams1.rect = rp.rect.ContractedBy(edgeDefenseWidth);
        resolveParams1.faction = faction;
        BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParams1);
        ResolveParams resolveParams2 = rp;
        resolveParams2.rect = rp.rect.ContractedBy(edgeDefenseWidth);
        resolveParams2.faction = faction;
        resolveParams2.skipSingleThingIfHasToWipeBuildingOrDoesntFit = false;
        SpawnSpecialThing(resolveParams2);
        ResolveParams resolveParams3 = rp;
        resolveParams3.rect = rp.rect.ContractedBy(edgeDefenseWidth);
        resolveParams3.faction = faction;
        resolveParams3.floorOnlyIfTerrainSupports = rp.floorOnlyIfTerrainSupports ?? true;
        BaseGen.symbolStack.Push("oagene_RuinedOutdoors", resolveParams3);
        ResolveParams resolveParams4 = rp;
        resolveParams4.floorDef = TerrainDefOf.Bridge;
        resolveParams4.floorOnlyIfTerrainSupports = rp.floorOnlyIfTerrainSupports ?? true;
        resolveParams4.allowBridgeOnAnyImpassableTerrain = rp.allowBridgeOnAnyImpassableTerrain ?? true;
        BaseGen.symbolStack.Push("floor", resolveParams4);

        if (ModsConfig.BiotechActive)
        {
            ResolveParams resolveParams5 = rp;
            resolveParams5.rect = rp.rect.ExpandedBy(Rand.Range(1, 4));
            resolveParams5.edgeUnpolluteChance = 0.5f;
            BaseGen.symbolStack.Push("unpollute", resolveParams5);
        }
    }
    private void SpawnSpecialThing(ResolveParams rp)
    {
        ResolveParams resolveParams1 = rp;
        resolveParams1.singleThingDef = Snowstrom_ThingDefOf.OAGene_Plant_SnowyCrystalTree_Seed;
        BaseGen.symbolStack.Push("thing", resolveParams1);
        ResolveParams resolveParams2 = rp;
        resolveParams2.singleThingDef = ThingDefOf.Campfire;
        resolveParams2.postThingGenerate = SpecialCampfire;
        BaseGen.symbolStack.Push("thing", resolveParams2);

        ResolveParams resolveParams3 = rp;
        resolveParams3.singleThingDef = Snowstrom_ThingDefOf.OAGene_AntiSnowTorch;
        BaseGen.symbolStack.Push("thing", resolveParams3);
        ResolveParams resolveParams4 = rp;
        resolveParams4.singleThingDef = Snowstrom_ThingDefOf.OAGene_AntiSnowTorch;
        BaseGen.symbolStack.Push("thing", resolveParams4);

        static void SpecialCampfire(Thing thing)
        {
            if (thing == null || thing.def != ThingDefOf.Campfire)
            {
                return;
            }
            ThingWithComps campfire = thing as ThingWithComps;
            campfire.GetComp<CompSpecialCampfire>()?.InitSpecialCampfire();
        }
    }
}
