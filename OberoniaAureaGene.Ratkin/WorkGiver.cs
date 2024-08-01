using RimWorld;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene.Ratkin;

public class WorkGiver_CarryToBuildingEnterable : WorkGiver_CarryToBuilding
{
    private ThingDef potentialThingDef;
    private ThingDef PotentialThingDef => potentialThingDef ??= def.GetModExtension<WorkGiverHaulToExtension>().thingDef;
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(PotentialThingDef);

}

public class WorkGiver_HaulToGeneDiscriminat : WorkGiver_Scanner
{
    private ThingDef potentialThingDef;
    private ThingDef PotentialThingDef => potentialThingDef ??= def.GetModExtension<WorkGiverHaulToExtension>().thingDef;
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(PotentialThingDef);

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t.IsForbidden(pawn))
        {
            return false;
        }
        if (!(t is Building_GeneDiscriminatorBase { GenepackLoaded: false } building_GeneDiscriminator))
        {
            return false;
        }
        if (!pawn.CanReserve(t, 1, -1, null, forced))
        {
            return false;
        }
        return building_GeneDiscriminator.targetGenepack != null;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!(t is Building_GeneDiscriminatorBase { GenepackLoaded: false } building_GeneDiscriminator))
        {
            return null;
        }
        Thing geneBank = building_GeneDiscriminator.GetTargetGeneBank();
        if (geneBank != null)
        {
            Job job = JobMaker.MakeJob(OAGene_RatkinDefOf.OAGene_HaulToDiscriminator, geneBank, building_GeneDiscriminator, building_GeneDiscriminator.targetGenepack);
            job.count = 1;
            return job;
        }
        return null;
    }
}
