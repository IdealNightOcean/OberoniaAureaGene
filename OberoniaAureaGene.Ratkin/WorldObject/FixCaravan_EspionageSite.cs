using OberoniaAurea_Frame;

namespace OberoniaAureaGene.Ratkin;

public class FixCaravan_EspionageSite : FixedCaravan
{
    protected override void TrySetAssociatedInterface()
    {
        associatedInterface = associatedWorldObject.GetComponent<EspionageSiteComp>()?.EspionageHandler;
    }
}
