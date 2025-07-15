using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class GameCondition_EndGame_ExtremeSnowstorm : GameCondition_ExtremeSnowstorm
{
    public const int DurationTick = 20 * 60000;

    public override Map MainMap
    {
        get
        {
            if (mainMap is null)
            {
                if (gameConditionManager.ownerMap is not null)
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
        try
        {
            GameComponent_Snowstorm.Instance.Notify_SnowstormStart();

            Map mainMap = MainMap;
            if (mainMap is not null)
            {
                causeColdSnap = TryAddFixedColdSnap(mainMap, DurationTick);
                InitExtremeSnowstorm_MainMap(mainMap, DurationTick);
            }

            for (int i = 0; i < AffectedMaps.Count; i++)
            {
                Map map = AffectedMaps[i];
                SnowstormUtility.InitExtremeSnowstorm_AllMaps(map, DurationTick);
            }

            Find.MusicManagerPlay.ForceTriggerTransition(Snowstorm_MiscDefOf.OAGene_Transition_Liebestraum);
        }
        catch
        {
            Log.Error("Attempt to initialize end-game extreme snowstorm failed.");
        }
    }

    protected override void PreEnd()
    {
        GameComponent_Snowstorm.Instance.Notify_SnowstormEnd();
        SnowstormUtility.EndExtremeSnowstorm_World();
        EndExtremeSnowstorm_MainMap(MainMap);
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.EndExtremeSnowstorm_AllMaps(map, slience: false);
        }
    }

    protected static bool TryAddFixedColdSnap(Map mainMap, int duration)
    {
        GameConditionManager gameConditionManager = mainMap.GameConditionManager;
        GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, duration);
        gameConditionManager.RegisterCondition(gameCondition);

        Letter letter = LetterMaker.MakeLetter("OAGene_LetterLabel_ExtremeSnowstormCauseColdSnap".Translate(), "OAGene_Letter_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
        Find.LetterStack.ReceiveLetter(letter, playSound: false);
        return false;
    }

    protected static void InitExtremeSnowstorm_MainMap(Map mainMap, int duration)
    {
        mainMap.SnowstormMapComp()?.Notify_SnowstormStart(DurationTick);
    }

    protected static void EndExtremeSnowstorm_MainMap(Map mainMap)
    {
        mainMap?.SnowstormMapComp()?.Notify_SnowstormEnd();
    }
    public void Notify_EndSnowstorm(int endDelay = 5000)
    {
        Duration = Find.TickManager.TicksGame - startTick + endDelay;
        Permanent = false;
    }

    public void Notify_QuestFailed(int endDelay = 120000)
    {
        Duration = Find.TickManager.TicksGame - startTick + endDelay;
        Permanent = false;
    }

}
