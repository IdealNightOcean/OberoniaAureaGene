using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Text;
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
    public override string GetInspectString()
    {
        StringBuilder stringBuilder = new(base.GetInspectString());
        stringBuilder.AppendInNewLine("OAGene_FixedCaravanEspionae_TimeLeft".Translate(ticksRemaining.ToStringTicksToPeriod()));
        return stringBuilder.ToString();
    }

    public override void Tick()
    {
        base.Tick();
        ticksRemaining--;
        if (ticksRemaining <= 0)
        {
            Caravan caravan = OAFrame_FixedCaravanUtility.ConvertToCaravan(this);
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
