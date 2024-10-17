using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class CompProperties_IceStormCrystal : CompProperties
{
    public float disappearHours;
    public CompProperties_IceStormCrystal()
    {
        compClass = typeof(CompIceStormCrystal);
    }
}
public class CompIceStormCrystal : ThingComp
{
    public CompProperties_IceStormCrystal Props => props as CompProperties_IceStormCrystal;
    public override void CompTickRare()
    {
        if (!parent.Spawned || parent.AmbientTemperature <= 0)
        {
            return;
        }
        int hitPointsAdjust = (int)((parent.MaxHitPoints / Props.disappearHours) * 0.1f);
        parent.HitPoints -= hitPointsAdjust;
        if (parent.HitPoints <= 0)
        {
            Map map = parent.Map;
            IntVec3 cell = parent.Position;
            Messages.Message("OAGene_IceStormCrystalMelted".Translate(), new LookTargets(cell, map), MessageTypeDefOf.NeutralEvent);
            parent.Destroy();
        }
    }
}
