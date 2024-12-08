using RimWorld;
using System.Collections.Generic;

namespace OberoniaAureaGene.Snowstorm;

public class StorytellerComp_RefiringSnowstormEndGame : StorytellerComp_RefiringUniqueQuest
{
    public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
    {
        if (!Snowstorm_StoryUtility.CanFireSnowstormEndGameNow())
        {
            yield break;
        }
        foreach (FiringIncident firingIncident in base.MakeIntervalIncidents(target))
        {
            yield return firingIncident;
        }
    }


}
