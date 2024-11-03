using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class Building_IceCrystalCollector : Building
{
    public static List<Building_IceCrystalCollector> CrystalCollectors = [];
    protected enum CurWeather
    {
        Other,
        Snowstorm,
        IceStorm,
        IceRain
    }
    protected const int MaxStorge = 20;

    [Unsaved]
    protected int nearCollectorCount;
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
        CrystalCollectors.Add(this);
        RecalculateNearCollector(this, despawn: false);
        underRoof = this.Map.roofGrid.Roofed(this.Position);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        CrystalCollectors.Remove(this);
        RecalculateNearCollector(this, despawn: true);
        base.DeSpawn(mode);
    }

    protected static void RecalculateNearCollector(Building_IceCrystalCollector parent, bool despawn = false)
    {
        Map map = parent.Map;
        IntVec3 position = parent.Position;
        IEnumerable<Building_IceCrystalCollector> mapCollectors = CrystalCollectors.Where(mapCollector);
        if (despawn)
        {
            foreach (Building_IceCrystalCollector collector in mapCollectors)
            {
                collector.nearCollectorCount = Math.Max(0, collector.nearCollectorCount - 1);
            }
            parent.nearCollectorCount = 0;
        }
        else
        {
            foreach (Building_IceCrystalCollector collector in mapCollectors)
            {
                parent.nearCollectorCount++;
                collector.nearCollectorCount++;
            }
        }

        bool mapCollector(Thing t)
        {
            if (t == parent || !t.Spawned)
            {
                return false;
            }
            if (t.Map != map)
            {
                return false;
            }
            if (t.Position.DistanceTo(position) > 14.9f)
            {
                return false;
            }
            return true;
        }
    }
    public override void TickLong()
    {
        base.TickLong();
        if (!this.Spawned)
        {
            curEfficiency = 0f;
            return;
        }
        if (this.Map.roofGrid.Roofed(this.Position))
        {
            underRoof = true;
            curEfficiency = 0f;
            return;
        }
        underRoof = false;
        CurWeather curWeather = GetCurWeather(this.Map.weatherManager.curWeather);
        float weatherEfficiency = curWeather switch
        {
            CurWeather.Other => -1f,
            CurWeather.Snowstorm => 5f,
            CurWeather.IceStorm => 25f,
            CurWeather.IceRain => 10f,
            _ => -1f,
        };
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
        if (thing != null)
        {
            GenPlace.TryPlaceThing(thing, base.Position, base.Map, ThingPlaceMode.Near);
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
        Thing thing = ThingMaker.MakeThing(Snowstrom_MiscDefOf.OAGene_IceCrystal);
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
            defaultLabel = "OAGene_IceCrystalCollector_CommandToggleUnloading".Translate(),
            defaultDesc = "OAGene_IceCrystalCollector_CommandToggleUnloadingDesc".Translate(this),
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
                defaultDesc = "OAGene_IceCrystalCollector_CommandEjectContentsDesc".Translate(Snowstrom_MiscDefOf.OAGene_IceCrystal.label),
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
        Scribe_Values.Look(ref unloadingEnabled, "unloadingEnabled", defaultValue:true);
       
    }

    protected static CurWeather GetCurWeather(WeatherDef weather)
    {
        if (weather == null)
        {
            return CurWeather.Other;
        }
        if (weather == Snowstrom_MiscDefOf.OAGene_SnowExtreme)
        {
            return CurWeather.Snowstorm;
        }
        if (weather == Snowstrom_MiscDefOf.OAGene_IceSnowExtreme)
        {
            return CurWeather.IceStorm;
        }
        if (weather == Snowstrom_MiscDefOf.OAGene_IceRain)
        {
            return CurWeather.IceRain;
        }
        return CurWeather.Other;
    }
}
