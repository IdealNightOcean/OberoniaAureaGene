using Verse;

namespace OberoniaAureaGene;

public class CompProperties_HegemonicFlag : CompProperties
{
    public CompProperties_HegemonicFlag()
    {
        compClass = typeof(CompHegemonicFlag);
    }
}
public class CompHegemonicFlag : ThingComp
{

    protected bool registed;
    MapComponent_OberoniaAureaGene oaGene_MCOAG;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        oaGene_MCOAG = parent.Map?.GetOAGeneMapComp();
        if (oaGene_MCOAG != null && !registed)
        {
            oaGene_MCOAG.HegemonicFlagCount++;
            registed = true;
        }
    }

    public override void PostDeSpawn(Map map)
    {
        if (oaGene_MCOAG != null && registed)
        {
            oaGene_MCOAG.HegemonicFlagCount--;
            registed = false;
        }
    }
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref registed, "registed", defaultValue: false);
    }
}
