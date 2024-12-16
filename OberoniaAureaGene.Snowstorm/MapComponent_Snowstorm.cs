using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class MapComponent_Snowstorm : MapComponent
{
    protected bool snowstormNow;
    protected bool snowstormFogNow;
    public bool SnowstormFogNow => snowstormFogNow;

    public List<Comp_SnowyCrystalTree> snowyCrystalTreeComps = [];
    public int SnowyCrystalTreeCount => snowyCrystalTreeComps.Count;

    public List<Building_IceCrystalCollector> crystalCollectors = [];

    public List<ThingWithComps> geothermalGenerators = [];
    public bool geothermalGeneratorWarned;
    protected static readonly IntRange GeothermalGeneratorInterval = new(8750, 11250);
    protected int geothermalGeneratorTicks = GeothermalGeneratorInterval.RandomInRange;

    public List<CompPowerPlant_ToxifierSnowstorm> toxifiers = [];
    public bool toxifierWarned;
    protected static readonly IntRange ToxifierInterval = new(8750, 11250);
    protected int toxifierTicks = ToxifierInterval.RandomInRange;

    public MapComponent_Snowstorm(Map map) : base(map) { }

    public void Notify_SnowstormStart(int duration)
    {
        if (snowstormNow)
        {
            return;
        }
        snowstormNow = true;
        map.weatherManager.TransitionTo(OAGene_MiscDefOf.OAGene_SnowExtreme);
        OAGeneUtility.TryBreakPowerPlantWind(map, duration);
        map.LongSnowstormMapComp()?.Notify_Snow(duration);
        geothermalGeneratorWarned = false;
        geothermalGeneratorTicks = 60000;
        toxifierWarned = false;
        toxifierTicks = 60000;
        Toxifier_NoticeSnowstorm(toxifiers, state: true);
    }
    public void Notify_SnowstormFog(bool state)
    {
        snowstormFogNow = state;
    }

    public void Notify_SnowstormEnd()
    {
        if (!snowstormNow)
        {
            return;
        }
        snowstormNow = false;
        map.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowHard);
        map.gameConditionManager.GetActiveCondition<GameCondition_SnowstormFog>()?.End();
        snowstormFogNow = false;
        Toxifier_NoticeSnowstorm(toxifiers, state: false);
    }

    public override void MapComponentTick()
    {
        if (snowstormNow)
        {
            geothermalGeneratorTicks--;
            if (geothermalGeneratorTicks < 0)
            {
                TryFireGeothermalGeneratorIncident();
                geothermalGeneratorTicks = GeothermalGeneratorInterval.RandomInRange;
            }

            toxifierTicks--;
            if (toxifierTicks < 0)
            {
                TryFireToxifierGeneratorIncident();
                toxifierTicks = ToxifierInterval.RandomInRange;
            }
        }
    }

    //冰晶收集装置
    public void Notyfy_CollectorSpawn(Building_IceCrystalCollector checker)
    {
        if (!crystalCollectors.Contains(checker))
        {
            RecalculateNearCollector(checker, false);
            crystalCollectors.Add(checker);
        }
    }
    public void Notyfy_CollectorDespawn(Building_IceCrystalCollector checker)
    {
        if (crystalCollectors.Contains(checker))
        {
            crystalCollectors.Remove(checker);
            RecalculateNearCollector(checker, true);
        }
    }
    protected void RecalculateNearCollector(Building_IceCrystalCollector checker, bool despawn = false)
    {
        Map map = base.map;
        IntVec3 position = checker.Position;
        IEnumerable<Building_IceCrystalCollector> mapCollectors = crystalCollectors.Where(mapCollector);
        if (despawn)
        {
            foreach (Building_IceCrystalCollector collector in mapCollectors)
            {
                collector.nearCollectorCount = Math.Max(0, collector.nearCollectorCount - 1);
            }
            checker.nearCollectorCount = 0;
        }
        else
        {
            foreach (Building_IceCrystalCollector collector in mapCollectors)
            {
                checker.nearCollectorCount++;
                collector.nearCollectorCount++;
            }
        }

        bool mapCollector(Thing t)
        {
            if (!t.Spawned)
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

    //地热发电机
    protected void TryFireGeothermalGeneratorIncident()
    {
        if (map.mapTemperature.OutdoorTemp > -30f || geothermalGenerators.Count == 0)
        {
            return;
        }
        if (!geothermalGeneratorWarned)
        {
            geothermalGeneratorWarned = true;
            Find.LetterStack.ReceiveLetter("OAGene_LetterLabel_WarnGeothermalGenerator".Translate(), "OAGene_Letter_WarnGeothermalGenerator".Translate(), LetterDefOf.NegativeEvent);
            return;
        }
        for (int i = 0; i < geothermalGenerators.Count; i++)
        {
            ThingWithComps generator = geothermalGenerators[i];
            if (generator == null)
            {
                continue;
            }
            if (RandomGeothermalGeneratorIncident(generator, map))
            {
                break;
            }
        }
    }
    protected static bool RandomGeothermalGeneratorIncident(ThingWithComps generator, Map map)
    {
        float value = Rand.Value;
        if (value < 0.6)
        {
            return false;
        }
        else if (value < 0.95)
        {
            LookTargets lookTargets = new(generator.Position, map);
            generator.TakeDamage(new DamageInfo(DamageDefOf.Crush, 75f));
            Messages.Message("OAGene_Message_WarnGeothermalGeneratorI".Translate(), lookTargets, MessageTypeDefOf.NegativeEvent, historical: false);
            return true;
        }
        else
        {
            LookTargets lookTargets = new(generator.Position, map);
            GenExplosion.DoExplosion(generator.Position, map, 1.1f, DamageDefOf.Bomb, instigator: null, damAmount: 10);
            if (!generator.Destroyed)
            {
                generator.TakeDamage(new DamageInfo(DamageDefOf.Crush, 190f));
                generator.GetComp<CompBreakdownable>()?.DoBreakdown();
            }
            Messages.Message("OAGene_Message_WarnGeothermalGeneratorII".Translate(), lookTargets, MessageTypeDefOf.NegativeEvent, historical: false);
            return true;
        }
    }

    //毒污发电机
    protected static void Toxifier_NoticeSnowstorm(List<CompPowerPlant_ToxifierSnowstorm> toxifiers, bool state)
    {
        for (int i = 0; i < toxifiers.Count; i++)
        {
            toxifiers[i]?.Notify_Snowstorm(state);
        }
    }
    protected void TryFireToxifierGeneratorIncident()
    {
        if (!ModsConfig.BiotechActive || map.mapTemperature.OutdoorTemp > -30f || toxifiers.Count == 0)
        {
            return;
        }
        if (!toxifierWarned)
        {
            toxifierWarned = true;
            Find.LetterStack.ReceiveLetter("OAGene_LetterLabel_WarnToxifier".Translate(), "OAGene_Letter_WarnToxifier".Translate(), LetterDefOf.NegativeEvent);
            return;
        }
        for (int i = 0; i < toxifiers.Count; i++)
        {
            CompPowerPlant_ToxifierSnowstorm toxifierComp = toxifiers[i];
            if (toxifierComp == null)
            {
                continue;
            }
            ThingWithComps toxifier = toxifierComp.parent;
            if (RandomToxifierGeneratorIncident(toxifier, map))
            {
                break;
            }
        }
    }
    protected static bool RandomToxifierGeneratorIncident(ThingWithComps toxifier, Map map)
    {
        if (Rand.Chance(0.25f))
        {
            LookTargets lookTargets = new(toxifier.Position, map);

            PolluteCells(toxifier.Position);
            FleckMaker.Static(toxifier.TrueCenter(), map, FleckDefOf.Fleck_ToxifierPollutionSource);
            SoundDefOf.Toxifier_Pollute.PlayOneShot(toxifier);

            float amount = toxifier.MaxHitPoints * 0.1f;
            toxifier.TakeDamage(new DamageInfo(DamageDefOf.Crush, amount));

            if (!toxifier.Destroyed)
            {
                GenExplosion.DoExplosion(toxifier.Position, map, 7.9f, DamageDefOf.ToxGas, instigator: null, damAmount: -1, postExplosionGasType: GasType.ToxGas);
            }

            Messages.Message("OAGene_Message_WarnToxifier".Translate(), lookTargets, MessageTypeDefOf.NegativeEvent, historical: false);
            return true;
        }
        return false;

        void PolluteCells(IntVec3 center)
        {
            int toralCell = GenRadial.NumCellsInRadius(26.9f);
            int pollutedCell = 0;
            for (int i = 0; i < toralCell; i++)
            {
                IntVec3 c = center + GenRadial.RadialPattern[i];
                if (c.InBounds(map) && c.CanPollute(map))
                {
                    map.pollutionGrid.SetPolluted(c, isPolluted: true, true);
                    pollutedCell++;
                    if (pollutedCell >= 16)
                    {
                        break;
                    }
                }
            }
        }
    }

    public override void MapGenerated()
    {
        if (SnowstormUtility.IsSnowExtremeWeather(map))
        {
            int duration = SnowstormUtility.SnowstormCondition?.TicksLeft ?? (7 * 60000);
            Notify_SnowstormStart(duration);
        }
    }
    public override void MapRemoved()
    {
        snowyCrystalTreeComps.Clear();
        crystalCollectors.Clear();
        geothermalGenerators.Clear();
        toxifiers.Clear();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref snowstormNow, "snowstormNow", defaultValue: false, forceSave: true);
        Scribe_Values.Look(ref snowstormFogNow, "snowstormFogNow", defaultValue: false);

        Scribe_Values.Look(ref geothermalGeneratorWarned, "geothermalGeneratorWarned", defaultValue: false);
        Scribe_Values.Look(ref geothermalGeneratorTicks, "geothermalGeneratorTicks", 0);

        Scribe_Values.Look(ref toxifierWarned, "toxifierWarned", defaultValue: false);
        Scribe_Values.Look(ref toxifierTicks, "toxifierTicks", 0);
    }

}
