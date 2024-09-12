using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class CompPowerNormalPlantWind : CompPowerPlantWind
{
    protected bool broken = true;
    protected int brokenRecoverTicks;

    protected override float DesiredPowerOutput => broken ? 0f : base.DesiredPowerOutput;
    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        GameCondition snowstorm = parent.Map?.gameConditionManager.GetActiveCondition(OberoniaAureaGeneDefOf.OAGene_ExtremeSnowstorm);
        if (snowstorm != null)
        {
            int duration = snowstorm.startTick + snowstorm.Duration - Find.TickManager.TicksGame;
            Notify_ExtremeSnowstorm(duration);
        }
    }
    public override void CompTick()
    {
        if (broken)
        {
            PowerOutput = 0f;
            brokenRecoverTicks--;
            if (brokenRecoverTicks <= 0)
            {
                broken = false;
            }
            return;
        }
        base.CompTick();
    }
    public void Notify_ExtremeSnowstorm(int duration)
    {
        brokenRecoverTicks = duration;
        broken = true;
        PowerOutput = 0f;
        breakdownableComp?.DoBreakdown();
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref broken, "broken", defaultValue: false);
        Scribe_Values.Look(ref brokenRecoverTicks, "brokenRecoverTicks", 0);
    }
}
