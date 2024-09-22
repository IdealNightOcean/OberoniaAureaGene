﻿using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class GameCondition_ExtremeSnowstorm : GameCondition_SnowstormBase
{
    protected static IntRange ColdGlowSpawnRange = new(30, 60);
    protected static IntRange ColdGlowIntervalRange = new(1200, 1500);
    protected static IntRange IceStormDelay = new(25000, 35000);

    public bool causeColdSnap;
    protected int coldGlowSpawnTicks;
    protected bool coldGlowSpawn;

    public override void Init()
    {
        base.Init();
        ColdSnapAndIceStorm();
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.InitExtremeSnowstorm(map, Duration);
        }
    }
    private void ColdSnapAndIceStorm()
    {
        if (Rand.Bool)
        {
            IncidentParms iceParms = new()
            {
                target = gameConditionManager.ownerMap
            };
            Find.Storyteller.incidentQueue.Add(OAGene_MiscDefOf.OAGene_ExtremeIceStorm, Find.TickManager.TicksGame + IceStormDelay.RandomInRange, iceParms);
        }
        if (Rand.Bool)
        {
            GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, this.Duration);
            gameConditionManager.RegisterCondition(gameCondition);
            Letter letter = LetterMaker.MakeLetter("OAGene_ExtremeSnowstormCauseColdSnapTitle".Translate(), "OAGene_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
            Find.LetterStack.ReceiveLetter(letter, playSound: false);
            Find.MusicManagerPlay.ForceTriggerTransition(OAGene_MiscDefOf.OAGene_Transition_ClairDeLune);
            causeColdSnap = true;
        }
    }
    public override void End()
    {
        base.End();
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.EndExtremeSnowstorm(map);
        }
    }
    public override void GameConditionTick()
    {
        coldGlowSpawnTicks--;
        if (coldGlowSpawnTicks < 0)
        {
            coldGlowSpawn = !coldGlowSpawn;
            coldGlowSpawnTicks = coldGlowSpawn ? ColdGlowSpawnRange.RandomInRange : ColdGlowIntervalRange.RandomInRange;
        }
    }
    public override void DoCellSteadyEffects(IntVec3 c, Map map)
    {
        if (!coldGlowSpawn)
        {
            return;
        }
        if (Rand.Chance(0.025f))
        {
            Vector3 fleckLoc = new(c.x + FastEffectRandom.Next(1, 50) / 100f, 10.54054f, c.z + FastEffectRandom.Next(1, 50) / 100f);
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(fleckLoc, map, OAGene_MiscDefOf.OAGene_ColdGlow, FastEffectRandom.Next(200, 300) / 100f);
            dataStatic.velocityAngle = FastEffectRandom.Next(0, 360);
            dataStatic.velocitySpeed = 0.04f;
            map.flecks.CreateFleck(dataStatic);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref causeColdSnap, "causeColdSnap", defaultValue: false);
    }
}
