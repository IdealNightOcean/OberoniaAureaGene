using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene.Snowstorm;

public class SitePartWorker_ConditionCauser_ClimateAdjusterCold : SitePartWorker_ConditionCauser
{
    public override void Notify_GeneratedByQuestGen(SitePart part, Slate slate, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
    {
        base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);
        CompCauseGameCondition_TemperatureOffset temperatureOffsetComp = part.conditionCauser.TryGetComp<CompCauseGameCondition_TemperatureOffset>();
        temperatureOffsetComp.SetTemperatureOffset(-10);
        string tempOffect = temperatureOffsetComp.temperatureOffset.ToStringTemperatureOffset();
        outExtraDescriptionRules.Add(new Rule_String("temperatureOffset", tempOffect));
    }
}
