using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class WeatherEvent_IceCrystalFlowerSpawn : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;
    public WeatherEvent_IceCrystalFlowerSpawn(Map map) : base(map) { }
    public override void WeatherEventTick() { }
    public override void FireEvent()
    {
        TryFireEvent(map);
        expired = true;
    }
    protected static void TryFireEvent(Map map)
    {
        if (Rand.Chance(0.75f))
        {
            return;
        }

        if (TryFindValidatorCell(map, out IntVec3 cell))
        {
            IceCrystalFlower flower = (IceCrystalFlower)GenSpawn.Spawn(Snowstrom_ThingDefOf.OAGene_Plant_IceCrystalFlower, cell, map);
            flower?.Notify_FirstSpawn();
            if (OAGene_SnowstormSettings.IceCrystalFlowerSpawnMessage)
            {
                Messages.Message("OAGene_MessagesIceCrystalFlowerSpawned".Translate(), flower, MessageTypeDefOf.NeutralEvent);
            }
        }
    }

    private static bool TryFindValidatorCell(Map map, out IntVec3 outCell)
    {
        for (int i = 0; i < 500; i++)
        {
            outCell = CellFinder.RandomCell(map);
            if (CellValidator(outCell))
            {
                return true;
            }
        }
        outCell = IntVec3.Invalid;
        return false;

        bool CellValidator(IntVec3 c)
        {
            if (c.Impassable(map))
            {
                return false;
            }
            if (c.GetThingList(map).Count != 0)
            {
                return false;
            }
            Room room = c.GetRoom(map);
            if (room == null || room.UsesOutdoorTemperature)
            {
                return false;
            }
            if (c.GetRoof(map)?.isThickRoof ?? false)
            {
                return true;
            }
            foreach (IntVec3 c1 in GenRadial.RadialCellsAround(c, 3.9f, false))
            {
                if (c1.InBounds(map) && c1.GetSnowDepth(map) > 0.1f)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
