using RimWorld.BaseGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class SymbolResolver_RuinedIndoors : SymbolResolver
{
    private const int MinLengthAfterSplit = 7;
    public override void Resolve(ResolveParams rp)
    {
        bool division;
        if (BaseGen.globalSettings.basePart_worshippedTerminalsResolved >= BaseGen.globalSettings.requiredWorshippedTerminalRooms)
        {
            division = rp.rect.Width > 13 || rp.rect.Height > 13 || ((rp.rect.Width >= 9 || rp.rect.Height >= 9) && Rand.Chance(0.3f));
        }
        else
        {
            division = (rp.rect.Width >= MinLengthAfterSplit * 2 && rp.rect.Height >= MinLengthAfterSplit * 2);
        }
        if (division)
        {
            ResolveParams resolveParams = rp;
            resolveParams.minLengthAfterSplit = MinLengthAfterSplit;
            BaseGen.symbolStack.Push("oagene_RuinedIndoors_Division", resolveParams);
        }
        else
        {
            BaseGen.symbolStack.Push("oagene_RuinedEmptyRoom", rp);
        }
    }
}
