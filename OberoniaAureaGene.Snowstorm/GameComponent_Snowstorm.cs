using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_Snowstorm : GameComponent
{
    protected int snowstormCount;
    public int lastSnowstormMentalTick = -1;

    public bool SnowstormNow => snowstormCount > 0;
    public GameComponent_Snowstorm(Game game) { }

    public void Notify_SnowstormStart()
    {
        snowstormCount++;
    }

    public void Notify_SnowstormEnd()
    {
        snowstormCount--;
        snowstormCount = snowstormCount > 0 ? snowstormCount : 0;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref snowstormCount, "snowstormCount", 0);
        Scribe_Values.Look(ref lastSnowstormMentalTick, "lastSnowstormMentalTick", -1);
    }
}
