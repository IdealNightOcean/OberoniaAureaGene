using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class ConditionalStatAffecter_TemperatureRange : ConditionalStatAffecterBase
{
    public float minTemperature = -99999f;
    public float maxTemperature = 99999f;

    public override bool Applies(StatRequest req)
    {
        if (req.HasThing && req.Thing is Pawn pawn)
        {
            float ambientTemperature = pawn.AmbientTemperature;
            return ambientTemperature > minTemperature && ambientTemperature <= maxTemperature;
        }
        return false;
    }
}

