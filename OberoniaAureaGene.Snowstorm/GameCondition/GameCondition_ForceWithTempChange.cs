using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_ForceWeatherWithTempChange : GameCondition_TemperatureChange
{
    public WeatherDef weather;

    public override void Init()
    {
        base.Init();
        weather ??= def.weatherDef;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref weather, "weather");
    }

    public override WeatherDef ForcedWeather()
    {
        return weather;
    }

    public override void RandomizeSettings(float points, Map map, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
    {
        base.RandomizeSettings(points, map, outExtraDescriptionRules, outExtraDescriptionConstants);
        weather = DefDatabase<WeatherDef>.AllDefsListForReading.Where((WeatherDef def) => def.isBad && def.canOccurAsRandomForcedEvent).RandomElement();
        outExtraDescriptionRules.AddRange(GrammarUtility.RulesForDef("forcedWeather", weather));
    }

}
