using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class MapComponent_LongSnowstorm : MapComponent
{
    private const int MaxSnowInterval = 5 * 60000;
    private static readonly IntRange ForeSnowDuration = new(60000, 120000);

    protected int lastSnowTick = -1;
    protected int snowCheckTicks;
    public MapComponent_LongSnowstorm(Map map) : base(map)
    { }

    public override void MapComponentTick()
    {
        snowCheckTicks--;
        if (snowCheckTicks <= 0)
        {
            CheckSnow();
        }
    }

    //漫长风雪的保底下雪天气
    protected void CheckSnow()
    {
        if (!map.IsPlayerHome || !map.GameConditionManager.ConditionIsActive(OAGene_MiscDefOf.OAGene_Snowstorm))
        {
            snowCheckTicks = MaxSnowInterval;
            return;
        }
        WeatherManager weatherManager = map.weatherManager;
        WeatherDef curWeather = weatherManager.curWeather;
        if (curWeather == OAGene_RimWorldDefOf.SnowHard || curWeather == OAGene_MiscDefOf.OAGene_SnowExtreme)
        {
            Notify_Snow(snowDuration: 2500, nextCheckDelay: 60000);
            return;
        }

        if (Find.TickManager.TicksGame - lastSnowTick > MaxSnowInterval && map.weatherDecider.ForcedWeather == null)
        {
            weatherManager.TransitionTo(OAGene_RimWorldDefOf.SnowHard);
            int duration = ForeSnowDuration.RandomInRange;
            OAFrame_ReflectionUtility.SetFieldValue(map.weatherDecider, "curWeatherDuration", duration);
            Notify_Snow(duration);
            return;
        }

        snowCheckTicks = 30000;
    }

    public void Notify_Snow(int snowDuration = 60000, int nextCheckDelay = -1)
    {
        lastSnowTick = Find.TickManager.TicksGame + snowDuration;
        if (nextCheckDelay < 0)
        {
            nextCheckDelay = snowDuration + 30000;
        }
        snowCheckTicks = nextCheckDelay;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref lastSnowTick, "lastSnowTick", -1);
        Scribe_Values.Look(ref snowCheckTicks, "snowCheckTicks", 0);
    }
}