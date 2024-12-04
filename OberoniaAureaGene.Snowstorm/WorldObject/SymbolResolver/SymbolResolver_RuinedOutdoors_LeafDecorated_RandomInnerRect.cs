using RimWorld.BaseGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class SymbolResolver_RuinedOutdoors_LeafDecorated_RandomInnerRect : SymbolResolver
{
    private const int MinLength = 5;

    private const int MaxRectSize = 15;

    public override bool CanResolve(ResolveParams rp)
    {
        if (base.CanResolve(rp) && rp.rect.Width <= MaxRectSize && rp.rect.Height <= MaxRectSize && rp.rect.Width > MinLength)
        {
            return rp.rect.Height > MinLength;
        }
        return false;
    }

    public override void Resolve(ResolveParams rp)
    {
        int width = Rand.RangeInclusive(MinLength, rp.rect.Width - 1);
        int height = Rand.RangeInclusive(MinLength, rp.rect.Height - 1);
        int xOffset = Rand.RangeInclusive(0, rp.rect.Width - width);
        int zOffset = Rand.RangeInclusive(0, rp.rect.Height - height);
        ResolveParams resolveParams = rp;
        resolveParams.rect = new CellRect(rp.rect.minX + xOffset, rp.rect.minZ + zOffset, width, height);
        BaseGen.symbolStack.Push("oagene_RuinedOutdoors_Leaf", resolveParams);
    }
}

