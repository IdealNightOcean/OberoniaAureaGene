using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class Building_IceCrystalCollector : Building
{
    protected const int MaxStorge = 25;

    [Unsaved]
    public int nearCollectorCount;
    public bool NearOtherCollector => nearCollectorCount > 0;
    [Unsaved]
    protected bool underRoof;

    public float CollectEfficiency => NearOtherCollector ? 0.05f : 1f;

    protected float curStorge;
    public float CurStorge => curStorge;
    protected float curEfficiency;

    public bool unloadingEnabled = true;
    public bool ReadyForHauling => Mathf.FloorToInt(curStorge) >= MaxStorge;
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        Snowstorm_MiscUtility.SnowstormMapComp(map)?.Notyfy_CollectorSpawn(this);

    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        Snowstorm_MiscUtility.SnowstormMapComp(Map)?.Notyfy_CollectorDespawn(this);
        base.DeSpawn(mode);
    }

    public override void TickLong()
    {
        base.TickLong();
        if (!Spawned)
        {
            curEfficiency = 0f;
            return;
        }
        if (Map.roofGrid.Roofed(Position))
        {
            underRoof = true;
            curEfficiency = 0f;
            return;
        }
        underRoof = false;
        float weatherEfficiency = GetCurWeatherEfficiency(Map.weatherManager.curWeather);
        if (weatherEfficiency < 0f)
        {
            curEfficiency = 0f;
            return;
        }
        curEfficiency = CollectEfficiency * weatherEfficiency;
        curStorge = Mathf.Min(curStorge + curEfficiency / 60f, MaxStorge);
    }
    private void EjectContents()
    {
        Thing thing = TakeOutBioferrite();
        if (thing is not null)
        {
            GenPlace.TryPlaceThing(thing, Position, Map, ThingPlaceMode.Near);
        }
    }

    public Thing TakeOutBioferrite()
    {
        int curStorgeInt = Mathf.FloorToInt(curStorge);
        if (curStorgeInt == 0)
        {
            return null;
        }
        curStorge -= curStorgeInt;
        Thing thing = ThingMaker.MakeThing(Snowstorm_ThingDefOf.OAGene_IceCrystal);
        thing.stackCount = curStorgeInt;
        return thing;
    }
    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (Gizmo gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }
        Command_Toggle command_Toggle = new()
        {
            defaultLabel = "OAGene_IceCrystalCollector_CommandToggleUnloading".Translate(Snowstorm_ThingDefOf.OAGene_IceCrystal.label),
            defaultDesc = "OAGene_IceCrystalCollector_CommandToggleUnloadingDesc".Translate(this, Snowstorm_ThingDefOf.OAGene_IceCrystal.label),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/BioferriteUnloading"),
            isActive = () => unloadingEnabled,
            toggleAction = delegate
            {
                unloadingEnabled = !unloadingEnabled;
            },
            activateSound = SoundDefOf.Tick_Tiny,

        };
        yield return command_Toggle;
        if (curStorge >= 1f)
        {
            Command_Action command_Eject = new()
            {
                defaultLabel = "OAGene_IceCrystalCollector_CommandEjectContents".Translate(),
                defaultDesc = "OAGene_IceCrystalCollector_CommandEjectContentsDesc".Translate(Snowstorm_ThingDefOf.OAGene_IceCrystal.label),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/EjectBioferrite"),
                action = EjectContents,
                Disabled = curStorge == 0f,
                activateSound = SoundDefOf.Tick_Tiny,

            };
            yield return command_Eject;
        }
        if (DebugSettings.ShowDevGizmos)
        {
            Command_Action command_DevAdd = new()
            {
                defaultLabel = "DEV: Add +1 ice crystal",
                action = delegate
                {
                    curStorge = Mathf.Min(curStorge + 1f, MaxStorge);
                }
            };
            yield return command_DevAdd;
        }
    }
    public override string GetInspectString()
    {
        StringBuilder sb = new(base.GetInspectString());
        sb.AppendInNewLine("OAGene_IceCrystalCollector_CurStorage".Translate(curStorge, MaxStorge));

        sb.AppendInNewLine("OAGene_IceCrystalCollector_CurEfficiency".Translate(curEfficiency));
        if (underRoof)
        {
            sb.AppendInNewLine("OAGene_IceCrystalCollector_UnderRoof".Translate(0f.ToStringPercent().Colorize(Color.red)));
        }
        if (NearOtherCollector)
        {
            sb.AppendInNewLine("OAGene_IceCrystalCollector_NearOtherCollector".Translate(0.05f.ToStringPercent().Colorize(Color.yellow)));
        }

        return sb.ToString();
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref curStorge, "curStorge", 0f);
        Scribe_Values.Look(ref curEfficiency, "curEfficiency", 0f);
        Scribe_Values.Look(ref unloadingEnabled, "unloadingEnabled", defaultValue: true);

    }

    protected static float GetCurWeatherEfficiency(WeatherDef weather)
    {
        if (weather is null)
        {
            return -1f;
        }
        if (weather == Snowstorm_MiscDefOf.OAGene_SnowExtreme)
        {
            return 8f;
        }
        if (weather == Snowstorm_MiscDefOf.OAGene_IceSnowExtreme)
        {
            return 30f;
        }
        if (weather == Snowstorm_MiscDefOf.OAGene_IceRain)
        {
            return 20f;
        }
        return -1f;
    }
}
