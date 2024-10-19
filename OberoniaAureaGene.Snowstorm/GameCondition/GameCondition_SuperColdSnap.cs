using RimWorld;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_SuperColdSnap : GameCondition
{
    private const float MaxTempOffset = -40f;

    public override int TransitionTicks => 12000;

    public override float TemperatureOffset()
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks, MaxTempOffset);
    }
}
