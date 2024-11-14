using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

internal class HediffComp_Protagonist : HediffComp
{
    protected bool expectationTrigged;
    protected bool obsessionTrigged;
    protected bool longCherishedTrigged;

    protected int ticksRemaining;

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            RecacheThought();
            ticksRemaining = 60000;
        }
    }

    protected void RecacheThought()
    {
        if (Current.Game.GetComponent<GameComponent_SnowstormStory>()?.StoryStart ?? true)
        {
            parent.pawn.health.RemoveHediff(parent);
            return;
        }
        float days = Find.TickManager.TicksGame.TicksToDays();
        if (days > 720)
        {

        }
        else if (days > 480)
        {

        }
        else if (days > 240)
        {

        }
    }

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        float days = Find.TickManager.TicksGame.TicksToDays();
        if (Current.Game.GetComponent<GameComponent_SnowstormStory>()?.StoryStart ?? true)
        {
            parent.pawn.health.RemoveHediff(parent);
            return;
        }
        if (days > 720)
        {
            expectationTrigged = true;
            obsessionTrigged = true;
        }
        else if (days > 480)
        {
            expectationTrigged = true;
        }
    }
}
