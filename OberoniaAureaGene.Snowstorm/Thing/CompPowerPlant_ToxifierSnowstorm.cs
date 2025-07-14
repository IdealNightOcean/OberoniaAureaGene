using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class CompPowerPlant_ToxifierSnowstorm : CompPowerPlant
{
    public bool snowstormNow;

    protected float OutdoorTemp => parent.Map?.mapTemperature.OutdoorTemp ?? 21f;

    protected override float DesiredPowerOutput
    {
        get
        {
            if (snowstormNow && OutdoorTemp < -30f)
            {
                return base.DesiredPowerOutput * 0.5f;
            }
            return base.DesiredPowerOutput;
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        MapComponent_Snowstorm snowstormMapComp = parent.Map.SnowstormMapComp();
        if (snowstormMapComp is not null)
        {
            if (!snowstormMapComp.toxifiers.Contains(this))
            {
                snowstormMapComp.toxifiers.Add(this);
            }
            if (SnowstormUtility.IsSnowExtremeWeather(parent.Map))
            {
                Notify_Snowstorm(state: true);
            }
        }
    }
    public override void PostDeSpawn(Map map)
    {
        Notify_Snowstorm(state: false);
        MapComponent_Snowstorm snowstormMapComp = map.SnowstormMapComp();
        if (snowstormMapComp is not null)
        {
            if (snowstormMapComp.toxifiers.Contains(this))
            {
                snowstormMapComp.toxifiers.Remove(this);
            }
        }
        base.PostDeSpawn(map);
    }
    public void Notify_Snowstorm(bool state)
    {
        snowstormNow = state;
    }
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref snowstormNow, "snowstormNow", defaultValue: false);
    }

}
