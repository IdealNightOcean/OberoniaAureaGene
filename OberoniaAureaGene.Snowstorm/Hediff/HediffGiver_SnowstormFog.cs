using OberoniaAurea_Frame;
using OberoniaAurea_Frame.Utility;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class HediffGiver_SnowstormFog : HediffGiver
{
    private readonly HediffGiveParams hediffGiveParams = new()
    {
        InitSeverity = 0.5f
    };

    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        if (ActiveHediff(pawn))
        {
            hediffGiveParams.HediffToGive = this.hediff;
            pawn.GetOrAddHediff(hediffGiveParams);
        }
    }
    public static bool ActiveHediff(Pawn p)
    {
        Map map = p.Map;
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        if (!map.SnowstormMapComp().SnowstormFogNow)
        {
            return false;
        }
        Room room = p.Position.GetRoom(map);
        if (room is not null)
        {
            return room.UsesOutdoorTemperature;
        }
        return true;
    }
}
