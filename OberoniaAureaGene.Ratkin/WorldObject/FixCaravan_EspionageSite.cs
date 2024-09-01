using OberoniaAurea_Frame;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class FixCaravan_EspionageSite : FixedCaravan
{
    public Site associateSite;
    public EspionageSiteComp associateEspionageSiteComp;
    public static readonly int ReconnaissanceTicks = 10000;

    public void SetEspionageSiteComp(Site site)
    {
        associateSite = site;
        associateEspionageSiteComp = associateSite?.GetComponent<EspionageSiteComp>();
    }

    public override void Tick()
    {
        base.Tick();
        ticksRemaining--;
        if (ticksRemaining <= 0)
        {
            Caravan caravan = FixedCaravanUtility.ConvertToCaravan(this);
            associateEspionageSiteComp?.TryGetOutCome(caravan);
        }
    }
    protected override void PreConvertToCaravanByPlayer()
    {
        associateEspionageSiteComp?.TryGetOutCome(null, true);
    }
    public override void Notify_ConvertToCaravan()
    { }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref associateSite, "associateSite");
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            associateEspionageSiteComp = associateSite?.GetComponent<EspionageSiteComp>();
        }
    }
}
