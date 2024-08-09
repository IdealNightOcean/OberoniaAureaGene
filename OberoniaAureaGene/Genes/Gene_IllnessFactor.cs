using Verse;

namespace OberoniaAureaGene;

public class Gene_IllnessFactor : Gene
{
    public float illnessFactor;
    public override void PostMake()
    {
        base.PostMake();
        illnessFactor = def.GetModExtension<GeneExtension>()?.illnessFactor ?? 1f;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            illnessFactor = def.GetModExtension<GeneExtension>()?.illnessFactor ?? 1f;
            //Log.Message("illnessFactor:" + illnessFactor);
        }
    }
}
