using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_FixedMechBreachingRaids : QuestNode
{
    public SlateRef<float> currentThreatPointsFactor = 1f;
    protected override bool TestRunInt(Slate slate)
    {
        Map hometownMap = slate.Get<Map>("hometownMap");
        return hometownMap != null;
    }
    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        Map hometownMap = slate.Get<Map>("hometownMap");
        if (hometownMap == null)
        {
            return;
        }

        IncidentParms parms = new()
        {
            target = hometownMap,
            faction = Faction.OfMechanoids,
            raidStrategy = Snowstrom_RimWorldDefOf.ImmediateAttackBreaching,
            points = StorytellerUtility.DefaultThreatPointsNow(hometownMap) * currentThreatPointsFactor.GetValue(slate)
        };
        Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + Rand.RangeInclusive(0, 60000), parms);
    }

}
