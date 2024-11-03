using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class CompProperties_IceCrystal : CompProperties
{
    public float disappearHours;
    public CompProperties_IceCrystal()
    {
        compClass = typeof(CompIceCrystal);
    }
}
public class CompIceCrystal : ThingComp
{
    public CompProperties_IceCrystal Props => props as CompProperties_IceCrystal;
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
            Messages.Message("OAGene_MessageIceCrystalMelted".Translate(), new LookTargets(cell, map), MessageTypeDefOf.NeutralEvent);
            parent.Destroy();
        }
    }
}
