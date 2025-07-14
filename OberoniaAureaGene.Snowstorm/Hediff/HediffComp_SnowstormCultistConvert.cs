using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffComp_SnowstormCultistConvert : HediffComp_SnowstormSpeech
{
    public HediffCompProperties_SnowstormSpeech Props => props as HediffCompProperties_SnowstormSpeech;

    protected const int ConvertInterval = 1200;

    [Unsaved]
    GameComponent_Snowstorm snowstormGameComp;
    GameComponent_Snowstorm SnowstormGameComp => snowstormGameComp ??= Snowstorm_MiscUtility.SnowstormGameComp;

    protected override void PostSpeechAction()
    {
        Pawn preacher = parent.pawn;
        if (preacher.Faction.IsPlayerSafe())
        {
            preacher.health.RemoveHediff(parent);
            return;
        }
        GameComponent_Snowstorm snowstormGameComp = SnowstormGameComp;
        if (snowstormGameComp is null)
        {
            preacher.health.RemoveHediff(parent);
            return;
        }
        if (!snowstormGameComp.CanCultistConvertNow)
        {
            return;
        }

        if (TryConvert(preacher.Map, preacher.Faction))
        {
            snowstormGameComp.nextCultistConvertTick = Find.TickManager.TicksGame + ConvertInterval;
        }
    }

    protected static bool TryConvert(Map map, Faction faction)
    {
        if (map is null)
        {
            return false;
        }
        bool convert = false;
        IEnumerable<Pawn> pawns = map.mapPawns.FreeColonistsSpawned.Where(p => p.Awake());
        if (pawns.Any())
        {
            convert = true;
            foreach (Pawn pawn in pawns)
            {
                pawn.needs.mood?.thoughts.memories.TryGainMemory(Snowstorm_ThoughtDefOf.OAGene_Thought_SnowstormCultistConvert);
                if (ModsConfig.IdeologyActive)
                {
                    float certaintyLoss = pawn.GetStatValue(StatDefOf.CertaintyLossFactor) * 0.025f * -1f;
                    pawn.ideo?.Reassure(certaintyLoss);
                }
            }
        }

        if (faction is null)
        {
            pawns = map.mapPawns.PrisonersOfColonySpawned.Where(p => p.Awake());
        }
        else
        {
            pawns = map.mapPawns.PrisonersOfColonySpawned.Where(p => p.Faction != faction && p.Awake());
        }

        if (pawns.Any())
        {
            convert = true;
            foreach (Pawn pawn in pawns)
            {
                pawn.needs.mood?.thoughts.memories.TryGainMemory(Snowstorm_ThoughtDefOf.OAGene_Thought_SnowstormCultistConvert);
                if (ModsConfig.IdeologyActive)
                {
                    float certaintyLoss = pawn.GetStatValue(StatDefOf.CertaintyLossFactor) * 0.025f * -1f;
                    pawn.ideo?.Reassure(certaintyLoss);
                }
            }
        }

        return convert;
    }
}