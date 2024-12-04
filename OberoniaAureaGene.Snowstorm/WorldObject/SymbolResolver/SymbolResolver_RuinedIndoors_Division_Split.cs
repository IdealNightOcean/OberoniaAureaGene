using RimWorld.BaseGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class SymbolResolver_RuinedIndoors_Division_Split : SymbolResolver_BasePart_Indoors_Division_Split
{
    private const int MinLengthAfterSplit = 5;

    private int ResolveMinLengthAfterSplit(ResolveParams rp)
    {
        return rp.minLengthAfterSplit ?? MinLengthAfterSplit;
    }

    private int ResolveMinWidthOrHeight(int minLengthAfterSplit)
    {
        return minLengthAfterSplit * 2 - 1;
    }

    public override bool CanResolve(ResolveParams rp)
    {
        int num = ResolveMinWidthOrHeight(ResolveMinLengthAfterSplit(rp));
        if (base.CanResolve(rp))
        {
            if (rp.rect.Width < num)
            {
                return rp.rect.Height >= num;
            }
            return true;
        }
        return false;
    }

    public override void Resolve(ResolveParams rp)
    {
        int length = ResolveMinLengthAfterSplit(rp);
        int rectSize = ResolveMinWidthOrHeight(length);
        if (rp.rect.Width < rectSize && rp.rect.Height < rectSize)
        {
            Log.Warning("Too small rect. params=" + rp);
        }
        else if ((Rand.Bool && rp.rect.Height >= rectSize) || rp.rect.Width < rectSize)
        {
            int indoorZOffset1 = Rand.RangeInclusive(length - 1, rp.rect.Height - length);
            ResolveParams resolveParams = rp;
            resolveParams.rect = new CellRect(rp.rect.minX, rp.rect.minZ, rp.rect.Width, indoorZOffset1 + 1);
            BaseGen.symbolStack.Push("oagene_RuinedIndoors", resolveParams);
            ResolveParams resolveParams2 = rp;
            resolveParams2.rect = new CellRect(rp.rect.minX, rp.rect.minZ + indoorZOffset1, rp.rect.Width, rp.rect.Height - indoorZOffset1);
            BaseGen.symbolStack.Push("oagene_RuinedIndoors", resolveParams2);
        }
        else
        {
            int indoorZOffset2 = Rand.RangeInclusive(length - 1, rp.rect.Width - length);
            ResolveParams resolveParams3 = rp;
            resolveParams3.rect = new CellRect(rp.rect.minX, rp.rect.minZ, indoorZOffset2 + 1, rp.rect.Height);
            BaseGen.symbolStack.Push("oagene_RuinedIndoors", resolveParams3);
            ResolveParams resolveParams4 = rp;
            resolveParams4.rect = new CellRect(rp.rect.minX + indoorZOffset2, rp.rect.minZ, rp.rect.Width - indoorZOffset2, rp.rect.Height);
            BaseGen.symbolStack.Push("oagene_RuinedIndoors", resolveParams4);
        }
    }
}
