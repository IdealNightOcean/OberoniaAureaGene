using RimWorld;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene.Snowstorm;
public class WorkGiver_TakeIceCrystalOutOfCollector : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Snowstrom_ThingDefOf.OAGene_IceCrystalCollector);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!(t is Building_IceCrystalCollector { unloadingEnabled: true, ReadyForHauling: true }))
        {
            return false;
        }
        if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
        {
            return false;
        }
        return true;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return JobMaker.MakeJob(Snowstrom_MiscDefOf.OAGene_Job_TakeIceCrystalOutOfCollector, t);
    }
}

