using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class Building_IceStormCrystalCollector : Building
{
    public static List<Building_IceStormCrystalCollector> CrystalCollectors = [];
    protected enum CurWeather
    {
        Other,
        Snowstorm,
        IceStorm,
        IceRain
    }
    protected const int Max_Storge = 20;

    protected int nearCollectorCount;
    public bool NearOtherCollector => nearCollectorCount > 0;

    public float CollectEfficiency => NearOtherCollector ? 0.05f : 1f;

    protected float curStorge;
    protected float curEfficiency;
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        CrystalCollectors.Add(this);
        RecalculateNearCollector(this, despawn: false);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        CrystalCollectors.Remove(this);
        RecalculateNearCollector(this, despawn: true);
        base.DeSpawn(mode);
    }

    protected static void RecalculateNearCollector(Building_IceStormCrystalCollector parent, bool despawn = false)
    {
        Map map = parent.Map;
        IntVec3 position = parent.Position;
        IEnumerable<Building_IceStormCrystalCollector> mapCollectors = CrystalCollectors.Where(mapCollector);
        if (despawn)
        {
            foreach (Building_IceStormCrystalCollector collector in mapCollectors)
            {
                collector.nearCollectorCount = Math.Max(0, collector.nearCollectorCount - 1);
            }
            parent.nearCollectorCount = 0;
        }
        else
        {
            foreach (Building_IceStormCrystalCollector collector in mapCollectors)
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
        if (!this.Spawned || this.Map.roofGrid.Roofed(this.Position))
        {
            curEfficiency = 0f;
            return;
        }
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
        curStorge = Mathf.Min(curStorge + curEfficiency / 60f, Max_Storge);
    }
    public override string GetInspectString()
    {
        StringBuilder sb = new(base.GetInspectString());
        sb.AppendInNewLine("OAGene_IceCrystalCollector_CurStorage".Translate(curStorge, Max_Storge));

        sb.AppendInNewLine("OAGene_IceCrystalCollector_CurEfficiency".Translate(curEfficiency));

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
