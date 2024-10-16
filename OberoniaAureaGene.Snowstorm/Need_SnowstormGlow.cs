using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class Need_SnowstormGlow : Need
{
    private const float NeedOffset = 0.9f * 0.0025f;
    public Need_SnowstormGlow(Pawn pawn) : base(pawn)
    {
        threshPercents = [0.1f];
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
            CurLevel -= NeedOffset;
        }
        else
        {
            CurLevel += NeedOffset;
        }
    }
    public override void OnNeedRemoved()
    {
        CurLevel = 1f;
        base.OnNeedRemoved();
    }
}
