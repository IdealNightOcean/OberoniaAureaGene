using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class ThoughtWorker_UnderDark : ThoughtWorker
{
    public const float MinLevelForThought = 0.1f;

    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        Need_SnowstormGlow need_SnowstormGlow = p.needs?.TryGetNeed<Need_SnowstormGlow>();
        return need_SnowstormGlow is not null && need_SnowstormGlow.CurLevel <= MinLevelForThought;
    }
}