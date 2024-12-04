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
        resolveParams2.floorOnlyIfTerrainSupports = rp.floorOnlyIfTerrainSupports ?? true;
        BaseGen.symbolStack.Push("oagene_RuinedOutdoors", resolveParams2);
        ResolveParams resolveParams3 = rp;
        resolveParams3.floorDef = TerrainDefOf.Bridge;
        resolveParams3.floorOnlyIfTerrainSupports = rp.floorOnlyIfTerrainSupports ?? true;
        resolveParams3.allowBridgeOnAnyImpassableTerrain = rp.allowBridgeOnAnyImpassableTerrain ?? true;
        BaseGen.symbolStack.Push("floor", resolveParams3);

        if (ModsConfig.BiotechActive)
        {
            ResolveParams resolveParams4 = rp;
            resolveParams4.rect = rp.rect.ExpandedBy(Rand.Range(1, 4));
            resolveParams4.edgeUnpolluteChance = 0.5f;
            BaseGen.symbolStack.Push("unpollute", resolveParams4);
        }
    }
}
