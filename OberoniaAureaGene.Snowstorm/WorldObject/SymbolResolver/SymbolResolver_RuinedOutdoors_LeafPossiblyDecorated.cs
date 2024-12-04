using RimWorld.BaseGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class SymbolResolver_RuinedOutdoors_LeafPossiblyDecorated : SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        ResolveParams resolveParams = rp;
        resolveParams.pathwayFloorDef = rp.pathwayFloorDef ?? BaseGenUtility.RandomBasicFloorDef(rp.faction);
        if (resolveParams.rect.Width >= 10 && resolveParams.rect.Height >= 10 && Rand.Chance(0.25f))
        {
            BaseGen.symbolStack.Push("oagene_RuinedOutdoors_LeafDecorated", resolveParams);
        }
        else
        {
            BaseGen.symbolStack.Push("oagene_RuinedOutdoors_Leaf", resolveParams);
        }
    }
}