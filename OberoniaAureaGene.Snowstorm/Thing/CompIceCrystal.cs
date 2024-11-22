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

    protected int cyclesToDie;
    public override void CompTickRare()
    {
        if (!parent.Spawned || parent.AmbientTemperature <= 0)
        {
            if (cyclesToDie > 0) { cyclesToDie--; }
            return;
        }
        cyclesToDie++;
        if (cyclesToDie >= Props.disappearHours * 10)
        {
            Map map = parent.Map;
            IntVec3 cell = parent.Position;
            Messages.Message("OAGene_MessageIceCrystalMelted".Translate(), new LookTargets(cell, map), MessageTypeDefOf.NeutralEvent);
            parent.Kill();
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref cyclesToDie, "cyclesToDie", 0);
    }
}
