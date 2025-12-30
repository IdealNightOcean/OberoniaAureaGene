using RimWorld;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_TemperatureChange : GameCondition
{
    public override int TransitionTicks => 6000;

    public override float TemperatureOffset()
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks, def.temperatureOffset);
    }
}
