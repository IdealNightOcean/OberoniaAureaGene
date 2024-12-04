using RimWorld.BaseGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class SymbolResolver_RuinedOutdoors : SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        bool division = (BaseGen.globalSettings.basePart_worshippedTerminalsResolved < BaseGen.globalSettings.requiredWorshippedTerminalRooms) ? (rp.rect.Width >= 14 && rp.rect.Height >= 14) : (rp.rect.Width > 23 || rp.rect.Height > 23 || ((rp.rect.Width >= 11 || rp.rect.Height >= 11) && Rand.Bool));
        ResolveParams resolveParams = rp;
        resolveParams.pathwayFloorDef = rp.pathwayFloorDef ?? BaseGenUtility.RandomBasicFloorDef(rp.faction);
        if (division)
        {
            BaseGen.symbolStack.Push("oagene_RuinedOutdoors_Division", resolveParams);
        }
        else
        {
            BaseGen.symbolStack.Push("oagene_RuinedOutdoors_LeafPossiblyDecorated", resolveParams);
        }
    }
}
