using Verse;

namespace OberoniaAureaGene;

public class Gene_PermanentChance : Gene
{
    public float becomePermanentChanceFactor;
    public override void PostMake()
    {
        base.PostMake();
        becomePermanentChanceFactor = def.GetModExtension<GeneExtension>()?.becomePermanentChanceFactor ?? 1f;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            becomePermanentChanceFactor = def.GetModExtension<GeneExtension>()?.becomePermanentChanceFactor ?? 1f;
        }
    }
}
