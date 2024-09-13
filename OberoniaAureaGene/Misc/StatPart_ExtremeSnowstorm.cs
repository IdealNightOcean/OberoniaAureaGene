using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class StatPart_ExtremeSnowstorm : StatPart
{
    private float multiplier = 1f;

    public override void TransformValue(StatRequest req, ref float val)
    {
        if (ActiveFor(req.Thing))
        {
            val *= multiplier;
        }
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (req.HasThing && ActiveFor(req.Thing))
        {
            return "StatsReport_MultiplierFor".Translate(OAGene_MiscDefOf.OAGene_ExtremeSnowstorm.label) + (": x" + multiplier.ToStringPercent());
        }
        return null;
    }

    protected static bool ActiveFor(Thing t)
    {
        if (t == null || !t.def.deteriorateFromEnvironmentalEffects)
        {
            return false;
        }
        Map map = t.MapHeld;
        IntVec3 cell = t.PositionHeld;
        if (map == null || !cell.IsValid)
        {
            return false;
        }
        if (map.gameConditionManager.ConditionIsActive(OAGene_MiscDefOf.OAGene_ExtremeSnowstorm))
        {
            return !cell.Roofed(map);
        }
        return false;
    }
}
