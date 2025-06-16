using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class QuestPart_EndGame_Success : QuestPart
{
    public string inSignal;
    public Pawn protagonist;
    public MapParent hometown;

    public override void Notify_QuestSignalReceived(Signal signal)
    {
        if (signal.tag == inSignal)
        {
            Map map = hometown.Map;
            CheckIfOnlyProtagonist(protagonist, map);
            Snowstorm_StoryUtility.StoryGameComp?.Notify_StroySuccess();
        }
    }
    protected static void CheckIfOnlyProtagonist(Pawn protagonist, Map map)
    {
        if (protagonist is null)
        {
            Snowstorm_StoryUtility.TryGetStoryProtagonist(out protagonist);
        }
        map ??= Snowstorm_StoryUtility.GetHometownMap();
        Snowstorm_StoryUtility.OnlyProtagonist = (map?.mapPawns.FreeColonistsSpawnedCount ?? 0) == 1;
        if (Snowstorm_StoryUtility.OnlyProtagonist)
        {
            Snowstorm_StoryUtility.OtherPawn = null;
        }
        else
        {
            Pawn tempPawn = protagonist.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Spouse);
            if (tempPawn is null || tempPawn.Map != map)
            {
                tempPawn = protagonist.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Lover);
            }
            if (tempPawn is null || tempPawn.Map != map)
            {
                tempPawn = map.mapPawns.FreeColonistsSpawned.Where(p => p != protagonist).RandomElementWithFallback();
            }

            if (tempPawn is null)
            {
                Snowstorm_StoryUtility.OnlyProtagonist = true;
                Snowstorm_StoryUtility.OtherPawn = null;
            }
            else
            {
                Snowstorm_StoryUtility.OtherPawn = tempPawn;
            }
        }
    }
    public override void Cleanup()
    {
        base.Cleanup();
        protagonist = null;
        hometown = null;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");

        Scribe_References.Look(ref protagonist, "protagonist");
        Scribe_References.Look(ref hometown, "hometown");
    }
}
