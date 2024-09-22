using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class StatPart_ExtremeSnowstorm : StatPart
{
    private float multiplier = 1f;
    private float corpseMultiplier = 1f;

    public override void TransformValue(StatRequest req, ref float val)
    {
        if (ActiveFor(req.Thing))
        {
            val *= req.Thing is Corpse ? corpseMultiplier : multiplier;
        }
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (ActiveFor(req.Thing))
        {
            float val = req.Thing is Corpse ? corpseMultiplier : multiplier;
            return "StatsReport_MultiplierFor".Translate(OAGene_MiscDefOf.OAGene_ExtremeSnowstorm.label) + (": x" + val.ToStringPercent());
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
        if (SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return !cell.Roofed(map);
        }
        return false;
    }
}
