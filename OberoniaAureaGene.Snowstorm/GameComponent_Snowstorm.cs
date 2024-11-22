using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_Snowstorm : GameComponent
{

    protected int snowstormCount;
    public int nextSnowstormMentalTick = -1;
    public bool CanGetSnowstormMentalNow => Find.TickManager.TicksGame > nextSnowstormMentalTick;
    public bool starryNightTriggered;

    public bool SnowstormNow => snowstormCount > 0;

    protected int totalSnowstormCount;

    public GameComponent_Snowstorm(Game game) { }


    public void Notify_SnowstormStart()
    {
        snowstormCount++;
        totalSnowstormCount++;
    }


    public void Notify_SnowstormEnd()
    {
        snowstormCount = Mathf.Max(snowstormCount - 1, 0);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref snowstormCount, "snowstormCount", 0);
        Scribe_Values.Look(ref nextSnowstormMentalTick, "nextSnowstormMentalTick", -1);

        Scribe_Values.Look(ref starryNightTriggered, "starryNightTriggered", defaultValue: false);
        Scribe_Values.Look(ref totalSnowstormCount, "totalSnowstormCount", 0);
    }
}
