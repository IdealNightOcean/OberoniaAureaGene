using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class WeatherEvent_IceCrystalFlowers : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;
    public WeatherEvent_IceCrystalFlowers(Map map) : base(map) { }
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

        if (CellFinder.TryFindRandomCell(map, CellValidator, out IntVec3 cell))
        {
            Thing flower = GenSpawn.Spawn(Snowstrom_MiscDefOf.OAGene_Plant_IceCrystalFlower, cell, map);
            Messages.Message("OAGene_MessagesIceCrystalFlowerSpawned".Translate(), flower, MessageTypeDefOf.NeutralEvent);
        }

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
            if (c.GetRoom(map) == null)
            {
                return false;
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
