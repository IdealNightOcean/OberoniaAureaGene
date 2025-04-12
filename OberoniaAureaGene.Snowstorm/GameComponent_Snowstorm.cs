using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameComponent_Snowstorm : GameComponent
{

    protected int snowstormCount;
    public int lastSnowstormStartTick = -1;
    public int lastSnowstormEndTick = -1;

    public int nextSnowstormMentalTick = -1;
    public int nextCultistConvertTick = -1;
    public bool CanGetSnowstormMentalNow => Find.TickManager.TicksGame > nextSnowstormMentalTick;
    public bool CanCultistConvertNow => Find.TickManager.TicksGame > nextCultistConvertTick;

    public bool starryNightTriggered;

    public bool SnowstormNow => snowstormCount > 0;

    protected int totalSnowstormCount;

    public GameComponent_Snowstorm(Game game) 
    {
        Snowstorm_MiscUtility.SnowstormGameComp = this;
    }

    public void Notify_SnowstormStart()
    {
        snowstormCount++;
        lastSnowstormStartTick = Find.TickManager.TicksGame;
        totalSnowstormCount++;
    }

    public void Notify_SnowstormEnd()
    {
        snowstormCount = Mathf.Max(snowstormCount - 1, 0);
        lastSnowstormEndTick = Find.TickManager.TicksGame;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref snowstormCount, "snowstormCount", 0);
        Scribe_Values.Look(ref lastSnowstormStartTick, "lastSnowstormStartTick", -1);
        Scribe_Values.Look(ref lastSnowstormEndTick, "lastSnowstormEndTick", -1);

        Scribe_Values.Look(ref nextSnowstormMentalTick, "nextSnowstormMentalTick", -1);
        Scribe_Values.Look(ref nextCultistConvertTick, "nextCultistConvertTick", -1);

        Scribe_Values.Look(ref starryNightTriggered, "starryNightTriggered", defaultValue: false);
        Scribe_Values.Look(ref totalSnowstormCount, "totalSnowstormCount", 0);
    }
}
