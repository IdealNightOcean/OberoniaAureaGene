using RimWorld;
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
            map.LongSnowstormMapComp()?.Notify_Snow(duration);
            OAGeneUtility.TryBreakPowerPlantWind(map, duration);
        }
    }
    protected void TryAddColdSnap()
    {
        if (Rand.Chance(0.3f))
        {
            GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, Duration);
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
            map.weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowHard);
            OAGeneUtility.TryGiveEndSnowstormThought(map);
        }
    }

    public override void GameConditionTick()
    {
        if (--coldGlowSpawnTicks < 0)
        {
            coldGlowSpawn = !coldGlowSpawn;
            coldGlowSpawnTicks = coldGlowSpawn ? ColdGlowSpawnRange.RandomInRange : ColdGlowIntervalRange.RandomInRange;
        }
    }

    public override void DoCellSteadyEffects(IntVec3 c, Map map)
    {
        if (coldGlowSpawn)
        {
            OAGeneUtility.SpawnColdGlowFleck(map: map, position: c, spawnChance: 0.025f, bigGlowSpawnChance: 0.05f);
        }
    }

    public override float MinWindSpeed() => 1f;

    public override float SkyGazeChanceFactor(Map map) => 0f;

    public override bool AllowEnjoyableOutsideNow(Map map) => false;

    public override float AnimalDensityFactor(Map map) => 0.25f;
    public override float PlantDensityFactor(Map map) => 0.1f;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref causeColdSnap, "causeColdSnap", defaultValue: false);
    }
}
