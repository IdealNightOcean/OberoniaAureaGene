using OberoniaAurea_Frame;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class HediffGiver_SnowstormFog : HediffGiver
{
    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        if (ActiveHediff(pawn))
        {
            pawn.AdjustOrAddHediff(hediff, -1, 250);
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
