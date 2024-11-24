using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WeatherEvent_IceCrystalsSpawn : WeatherEvent
{
    public bool expired;
    public override bool Expired => expired;

    protected static readonly IntRange CrystalsCountRange = new(8, 20);

    public WeatherEvent_IceCrystalsSpawn(Map map) : base(map) { }
    public override void WeatherEventTick() { }

    public override void FireEvent()
    {
        TryFireEvent(map);
        expired = true;
    }
    protected static void TryFireEvent(Map map)
    {
        IntVec3 spawnCenter = DropCellFinder.RandomDropSpot(map);
        if (!spawnCenter.IsValid)
        {
            return;
        }
        List<Thing> spawnThings = OAFrame_MiscUtility.TryGenerateThing(Snowstrom_MiscDefOf.OAGene_IceCrystal, CrystalsCountRange.RandomInRange);
        for (int i = 0; i < spawnThings.Count; i++)
        {
            Thing t = spawnThings[i];
            CompForbiddable forbiddable = t.TryGetComp<CompForbiddable>();
            if (forbiddable != null)
            {
                forbiddable.Forbidden = true;
            }
            GenPlace.TryPlaceThing(t, spawnCenter, map, ThingPlaceMode.Near, delegate (Thing thing, int count)
            {
                PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
            }, null, t.def.defaultPlacingRot);
        }
        Messages.Message("OAGene_MessageIceCrystalsSpawn".Translate(), new LookTargets(spawnCenter, map), MessageTypeDefOf.NeutralEvent);
    }
}
