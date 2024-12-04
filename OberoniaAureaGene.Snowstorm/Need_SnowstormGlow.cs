using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class Need_SnowstormGlow : Need
{
    private const float ReduceOffset = 5f / 6f * 0.00625f;
    private const float IncreaseOffset = 5f / 6f * 0.025f;
    public Need_SnowstormGlow(Pawn pawn) : base(pawn)
    {
        threshPercents = [0.1f, 0.35f, 0.7f];
    }

    public override void SetInitialLevel()
    {
        CurLevel = 1f;
    }

    public override void NeedInterval()
    {
        if (IsFrozen || !pawn.Spawned)
        {
            return;
        }
        if (pawn.Map.glowGrid.GroundGlowAt(pawn.Position) < 0.5f)
        {
            CurLevel -= ReduceOffset;
        }
        else
        {
            CurLevel += IncreaseOffset;
        }
    }
    public override void OnNeedRemoved()
    {
        CurLevel = 1f;
        base.OnNeedRemoved();
    }
}
