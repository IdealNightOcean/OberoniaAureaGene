using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class GameCondition_EndGame_ExtremeSnowstorm : GameCondition_ExtremeSnowstorm
{
    protected const int DurationTick = 20 * 60000;

    public override Map MainMap
    {
        get
        {
            if (mainMap == null)
            {
                if (gameConditionManager.ownerMap != null)
                {
                    mainMap = gameConditionManager.ownerMap;
                }
                else
                {
                    MapParent hometown = Find.WorldObjects.AllWorldObjects.Where(o => o.def == Snowstrom_MiscDefOf.OAGene_Hometown).FirstOrFallback() as MapParent;
                    if (hometown != null)
                    {
                        mainMap = hometown.Map;
                    }
                }
            }
            return mainMap;
        }
    }
    protected override void PostInit()
    {
        Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormStart();
        Duration = DurationTick;
        Permanent = true;
        AddFixedColdSnap();
        Map mainMap = MainMap;
        if (mainMap != null)
        {
            mainMap.SnowstormMapComp()?.Notify_SnowstromStart();
            AddFixedIncident(mainMap);
        }
    }

    protected override void PreEnd()
    {
        Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormEnd();
        MainMap?.SnowstormMapComp()?.Notify_SnowstromEnd();
    }

    protected void AddFixedColdSnap()
    {
        if (MainMap != null)
        {
            Map mainMap = MainMap;
            if (mainMap != null)
            {
                if (OAGene_RimWorldDefOf.ColdSnap.Worker.CanFireNow(new IncidentParms { target = mainMap }))
                {
                    GameConditionManager gameConditionManager = mainMap.GameConditionManager;
                    GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, Duration);
                    gameConditionManager.RegisterCondition(gameCondition);

                    Letter letter = LetterMaker.MakeLetter("OAGene_LetterLabel_ExtremeSnowstormCauseColdSnap".Translate(), "OAGene_Letter_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
                    Find.LetterStack.ReceiveLetter(letter, playSound: false);
                    Find.MusicManagerPlay.ForceTriggerTransition(OAGene_MiscDefOf.OAGene_Transition_ClairDeLune);
                    causeColdSnap = true;
                }
            }
        }
    }

    public static void AddFixedIncident(Map map)
    {
        SnowstormUtility.AddNewMapIncident(Snowstrom_IncidentDefOf.OAGene_CommunicationTowerCollapse, map, DayToDelaytick(3));

        SnowstormUtility.AddNewMapIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(3));
        SnowstormUtility.AddNewMapIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(6));
        SnowstormUtility.AddNewMapIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(12));
        SnowstormUtility.AddNewMapIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(16));
        SnowstormUtility.AddNewMapIncident(Snowstrom_IncidentDefOf.OAGene_SnowstormCold, map, DayToDelaytick(18));

    }
    public void Notify_EndGame()
    {
        Permanent = false;
    }

    private static int DayToDelaytick(int day)
    {
        return (day - 1) * 60000 + Rand.RangeInclusive(0, 60000);
    }
}
