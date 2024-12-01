using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class GameCondition_EndGame_ExtremeSnowstorm : GameCondition_ExtremeSnowstormBase
{
    protected const int DurationTick = 20 * 60000;

    protected override void PostInit()
    {
        Duration = DurationTick;
        Permanent = true;
        AddFixedColdSnap();
        Map mainMap = gameConditionManager.ownerMap;
        AddFixedIncident(mainMap);
    }

    protected void AddFixedColdSnap()
    {
        GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, this.Duration);
        gameConditionManager.RegisterCondition(gameCondition);
        Letter letter = LetterMaker.MakeLetter("OAGene_ExtremeSnowstormCauseColdSnapTitle".Translate(), "OAGene_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
        Find.LetterStack.ReceiveLetter(letter, playSound: false);
        Find.MusicManagerPlay.ForceTriggerTransition(OAGene_MiscDefOf.OAGene_Transition_ClairDeLune);
        causeColdSnap = true;
    }

    public static void AddFixedIncident(Map map)
    {
        SnowstormUtility.AddNewIncident(Snowstrom_IncidentDefOf.OAGene_CommunicationTowerCollapse, map, DayToDelaytick(5));

        SnowstormUtility.AddNewIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(2));
        SnowstormUtility.AddNewIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(10));
        SnowstormUtility.AddNewIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(16));
        SnowstormUtility.AddNewIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(18));

    }

    private static int DayToDelaytick(int day)
    {
        return (day - 1) * 60000 + Rand.RangeInclusive(0, 60000);
    }
    public static void AddNewSnowstromRaid(IncidentDef incidentDef, Map targetMap, int delayTicks)
    {
        IncidentParms parms = new()
        {
            target = targetMap,
            points = Rand.RangeInclusive(8000, 10000)
        };
        Find.Storyteller.incidentQueue.Add(incidentDef, Find.TickManager.TicksGame + delayTicks, parms);
    }
}
