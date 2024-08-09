using OberoniaAurea_Frame;
using RimWorld.Planet;

namespace OberoniaAureaGene.Ratkin;

public class FixCaravan_EspionageSite : FixedCaravan
{
    Site associateSite;
    public EspionageSiteComp AssociateEspionageSiteComp => associateSite?.GetComponent<EspionageSiteComp>();
    public static readonly int ReconnaissanceTicks = 10000;

    public override void Tick()
    {
        base.Tick();
        ticksRemaining--;
        if (ticksRemaining <= 0)
        {
            Caravan caravan = FixedCaravanUtility.ConvertToCaravan(this);
            AssociateEspionageSiteComp?.TryGetOutCome(caravan);
        }
    }
    protected override void PreConvertToCaravanByPlayer()
    {
        AssociateEspionageSiteComp?.ForceFail();
    }
    public override void Notify_ConvertToCaravan()
    { }
}
