using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GenStep_Hometown : GenStep_Scatterer
{
    private static readonly IntRange SettlementSizeRange = new(34, 38);

    public bool clearBuildingFaction;

    // private static readonly List<IntVec3> tmpCandidates = [];

    public override int SeedPart => 1002728781;

    protected override bool CanScatterAt(IntVec3 c, Map map)
    {
        if (!base.CanScatterAt(c, map))
        {
            return false;
        }
        if (!c.Standable(map))
        {
            return false;
        }
        if (c.Roofed(map))
        {
            return false;
        }
        if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors)))
        {
            return false;
        }
        int min = SettlementSizeRange.min;
        if (!new CellRect(c.x - min / 2, c.z - min / 2, min, min).FullyContainedWithin(new CellRect(0, 0, map.Size.x, map.Size.z)))
        {
            return false;
        }
        return true;
    }

    protected override void ScatterAt(IntVec3 c, Map map, GenStepParams parms, int stackCount = 1)
    {
        int width = SettlementSizeRange.RandomInRange;
        int height = SettlementSizeRange.RandomInRange;
        CellRect rect = new(c.x - width / 2, c.z - height / 2, width, height);
        Faction faction = Faction.OfPlayer;
        rect.ClipInsideMap(map);
        ResolveParams resolveParams = default;
        resolveParams.rect = rect;
        resolveParams.faction = faction;
        resolveParams.settlementDontGeneratePawns = true;
        BaseGen.globalSettings.map = map;
        BaseGen.globalSettings.minBuildings = 1;
        BaseGen.globalSettings.minBarracks = 1;
        BaseGen.symbolStack.Push("oagene_Hometown", resolveParams);

        BaseGen.Generate();

        if (!clearBuildingFaction)
        {
            return;
        }
        foreach (Building item in map.listerThings.GetThingsOfType<Building>())
        {
            if (item is not Building_Turret && item.Faction == faction)
            {
                item.SetFaction(null);
            }
        }
    }
}
