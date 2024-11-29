using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class GameCondition_ExtremeSnowstormBase : GameCondition_SnowstormBase
{
    protected static IntRange ColdGlowSpawnRange = new(30, 60);
    protected static IntRange ColdGlowIntervalRange = new(1200, 1500);

    public bool causeColdSnap;

    protected int coldGlowSpawnTicks;
    protected bool coldGlowSpawn;

    public override void Init()
    {
        base.Init();
        PostInit();
    }

    protected virtual void PostInit()
    {
        TryAddColdSnap();
        int duration = Duration;
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            map.weatherManager.TransitionTo(OAGene_MiscDefOf.OAGene_SnowExtreme);
            map.GetOAGeneMapComp()?.Notify_Snow(duration);
            OAGeneUtility.TryBreakPowerPlantWind(map, duration);
        }
    }
    protected void TryAddColdSnap()
    {
        if (Rand.Chance(0.3f))
        {
            GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, this.Duration);
            gameConditionManager.RegisterCondition(gameCondition);
            Letter letter = LetterMaker.MakeLetter("OAGene_LetterLabel_ExtremeSnowstormCauseColdSnap".Translate(), "OAGene_Letter_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
            Find.LetterStack.ReceiveLetter(letter, playSound: false);
            Find.MusicManagerPlay.ForceTriggerTransition(OAGene_MiscDefOf.OAGene_Transition_ClairDeLune);
            causeColdSnap = true;
        }
    }

    public override void End()
    {
        PreEnd();
        base.End();
    }
    protected virtual void PreEnd()
    {
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            map.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowGentle);
            OAGeneUtility.TryGiveEndSnowstormThought(map);
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
        if (Random.value < 0.025f)
        {
            FleckDef fleckDef = Rand.Chance(0.95f) ? OAGene_MiscDefOf.OAGene_ColdGlow : OAGene_MiscDefOf.OAGene_BigColdGlow;
            Vector3 fleckLoc = new(c.x + FastEffectRandom.Next(1, 50) / 100f, 10.54054f, c.z + FastEffectRandom.Next(1, 50) / 100f);
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(fleckLoc, map, fleckDef, FastEffectRandom.Next(200, 300) / 100f);
            dataStatic.velocityAngle = FastEffectRandom.Next(0, 360);
            dataStatic.velocitySpeed = 0.08f;
            map.flecks.CreateFleck(dataStatic);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref causeColdSnap, "causeColdSnap", defaultValue: false);
    }
}
