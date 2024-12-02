using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_FixedSnowstormBreachingRaids : QuestNode
{

    public SlateRef<float> currentThreatPointsFactor = 1f;
    protected override bool TestRunInt(Slate slate)
    {
        return true;
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
            points = StorytellerUtility.DefaultThreatPointsNow(hometownMap) * currentThreatPointsFactor.GetValue(slate)
        };
        Find.Storyteller.incidentQueue.Add(Snowstrom_IncidentDefOf.OAGene_SnowstormMaliceRaid, Find.TickManager.TicksGame + Rand.RangeInclusive(0, 60000), parms);
    }

}
