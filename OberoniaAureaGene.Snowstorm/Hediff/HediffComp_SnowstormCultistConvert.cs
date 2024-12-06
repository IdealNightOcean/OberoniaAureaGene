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
        GameComponent_Snowstorm snowstormGameComp = SnowstormGameComp;
        if (snowstormGameComp == null || !snowstormGameComp.CanCultistConvertNow)
        {
            return;
        }
        if (TryConvert(parent.pawn))
        {
            snowstormGameComp.nextCultistConvertTick = Find.TickManager.TicksGame + ConvertInterval;
        }
    }

    protected static bool TryConvert(Pawn preacher)
    {
        IEnumerable<Pawn> pawns = preacher.Map?.mapPawns.FreeColonistsSpawned.Where(p => p.Awake());
        if (pawns == null || !pawns.Any())
        {
            return false;
        }
        foreach (Pawn pawn in pawns)
        {
            pawn.needs.mood?.thoughts.memories.TryGainMemory(Snowstorm_ThoughtDefOf.OAGene_Thought_SnowstormCultistConvert);
            if (ModsConfig.IdeologyActive)
            {
                float certaintyLoss = pawn.GetStatValue(StatDefOf.CertaintyLossFactor) * 0.025f * -1f;
                pawn.ideo?.Reassure(certaintyLoss);
            }
        }
        return true;
    }
}