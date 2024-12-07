using RimWorld;
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
                    mainMap = Snowstorm_StoryUtility.GetHometownMap();
                }
            }
            return mainMap;
        }
    }
    protected override void PostInit()
    {
        Duration = DurationTick;
        Permanent = true;
        Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormStart();

        Map mainMap = MainMap;
        if (mainMap != null)
        {
            causeColdSnap = TryAddFixedColdSnap(mainMap, DurationTick);
            InitExtremeSnowstorm_MainMap(mainMap, DurationTick);
        }

        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.InitExtremeSnowstorm_AllMaps(map, DurationTick);
        }
    }

    protected override void PreEnd()
    {
        Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormEnd();
        EndExtremeSnowstorm_MainMap(MainMap);
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.EndExtremeSnowstorm_AllMaps(map, slience: false);
        }
    }

    protected static bool TryAddFixedColdSnap(Map mainMap, int duration)
    {
        if (OAGene_RimWorldDefOf.ColdSnap.Worker.CanFireNow(new IncidentParms { target = mainMap }))
        {
            GameConditionManager gameConditionManager = mainMap.GameConditionManager;
            GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, duration);
            gameConditionManager.RegisterCondition(gameCondition);

            Letter letter = LetterMaker.MakeLetter("OAGene_LetterLabel_ExtremeSnowstormCauseColdSnap".Translate(), "OAGene_Letter_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
            Find.LetterStack.ReceiveLetter(letter, playSound: false);
            Find.MusicManagerPlay.ForceTriggerTransition(OAGene_MiscDefOf.OAGene_Transition_ClairDeLune);
        }
        return false;
    }

    protected static void InitExtremeSnowstorm_MainMap(Map mainMap, int duration)
    {
        mainMap.SnowstormMapComp()?.Notify_SnowstormStart(DurationTick);
    }

    protected static void EndExtremeSnowstorm_MainMap(Map mainMap)
    {
        if (mainMap != null)
        {
            mainMap.SnowstormMapComp()?.Notify_SnowstormEnd();
        }
    }
    public void Notify_EndGame()
    {
        Permanent = false;
    }

    public void Notify_QuestFailed()
    {
        Duration = Find.TickManager.TicksGame - startTick + 120000;
        Permanent = false;
    }

}
