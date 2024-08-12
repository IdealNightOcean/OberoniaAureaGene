using OberoniaAurea_Frame;
using RimWorld.Planet;

namespace OberoniaAureaGene.Ratkin;

public class FixCaravan_EspionageSite : FixedCaravan
{
    public EspionageSiteComp associateEspionageSiteComp;
    public static readonly int ReconnaissanceTicks = 10000;

    public void SetEspionageSiteComp(EspionageSiteComp comp)
    {
        associateEspionageSiteComp = comp;
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
        associateEspionageSiteComp?.ForceFail();
    }
    public override void Notify_ConvertToCaravan()
    { }
}
