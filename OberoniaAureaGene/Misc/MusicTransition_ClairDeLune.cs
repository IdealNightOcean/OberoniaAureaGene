using RimWorld;

namespace OberoniaAureaGene;

public class MusicTransition_OnlyPlayOneTime : MusicTransition
{
    public override bool IsTransitionSatisfied()
    {
        return false;
    }
}
