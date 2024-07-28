using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class WorkGiver_CarryToBuildingEnterable : WorkGiver_CarryToBuilding
{
    private ThingDef potentialThingDef;
    private ThingDef PotentialThingDef => potentialThingDef ??= def.GetModExtension<WorkGiverHaulToExtension>().thingDef;
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(PotentialThingDef);

}
