using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.QuestGen;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_FixedMechBreachingRaids : QuestNode_FireIncident
{
    protected override IncidentParms ResolveParms(Slate slate)
    {
        return new()
        {
            forced = true,
            faction = Faction.OfMechanoids,
            raidStrategy = Snowstorm_RimWorldDefOf.ImmediateAttackBreaching,
        };
    }
}
