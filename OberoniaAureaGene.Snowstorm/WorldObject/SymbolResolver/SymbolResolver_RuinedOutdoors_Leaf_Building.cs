using RimWorld.BaseGen;

namespace OberoniaAureaGene.Snowstorm;

public class SymbolResolver_RuinedOutdoors_Leaf_Building : SymbolResolver_BasePart_Outdoors_Leaf_Building
{
    public override void Resolve(ResolveParams rp)
    {
        ResolveParams resolveParams = rp;
        resolveParams.wallStuff = rp.wallStuff ?? BaseGenUtility.RandomCheapWallStuff(rp.faction);
        resolveParams.floorDef = rp.floorDef ?? BaseGenUtility.RandomBasicFloorDef(rp.faction, allowCarpet: true);
        BaseGen.symbolStack.Push("oagene_RuinedIndoors", resolveParams);
        BaseGen.globalSettings.basePart_buildingsResolved++;
    }
}
