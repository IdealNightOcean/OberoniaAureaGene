using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene.Ratkin;


[StaticConstructorOnStartup]
public static class GetTradeThings_Patch
{
    private static readonly HashSet<Thing> YieldedThings = [];

    static GetTradeThings_Patch()
    {
        if (ModLister.GetActiveModWithIdentifier(identifier: "OARK.RatkinFaction.OberoniaAurea", ignorePostfix: true) is null)
            return;

        ModHarmonyPatch.HarmonyInstance.Patch(original: AccessTools.Method(typeof(TradeUtility), nameof(TradeUtility.AllLaunchableThingsForTrade)),
                                              prefix: null,
                                              postfix: new HarmonyMethod(typeof(GetTradeThings_Patch), nameof(AllLaunchableThingsForTrade_Postfix)));

        ModHarmonyPatch.HarmonyInstance.Patch(original: AccessTools.Method(typeof(Pawn_TraderTracker), nameof(Pawn_TraderTracker.ColonyThingsWillingToBuy)),
                                              prefix: null,
                                              postfix: new HarmonyMethod(typeof(GetTradeThings_Patch), nameof(ColonyThingsWillingToBuy_Postfix)));

    }

    public static IEnumerable<Thing> AllLaunchableThingsForTrade_Postfix(IEnumerable<Thing> originValue, Map map, ITrader trader = null)
    {
        foreach (Thing t in originValue)
            yield return t;

        YieldedThings.Clear();
        foreach (Building_OrbitalTradeBeacon item in Building_OrbitalTradeBeacon.AllPowered(map))
        {
            foreach (IntVec3 tradeableCell in item.TradeableCells)
            {
                IEnumerable<Thing> geneBanks = tradeableCell.GetThingList(map).Where(b => b.def == OAGene_RatkinDefOf.OAGene_OAGeneBank);
                foreach (Thing bank in geneBanks)
                {
                    CompGenepackContainer compGenepackContainer = bank.TryGetComp<CompGenepackContainer>();
                    if (compGenepackContainer is null)
                        continue;

                    List<Genepack> containedGenepacks = compGenepackContainer.ContainedGenepacks;
                    foreach (Genepack pack in containedGenepacks)
                    {
                        if (TradeUtility.PlayerSellableNow(bank, trader) && !YieldedThings.Contains(pack))
                        {
                            YieldedThings.Add(pack);
                            yield return pack;
                        }
                    }
                }
            }
        }
        YieldedThings.Clear();
    }

    public static IEnumerable<Thing> ColonyThingsWillingToBuy_Postfix(IEnumerable<Thing> originValue, Pawn ___pawn, Pawn playerNegotiator)
    {
        foreach (Thing t in originValue)
            yield return t;

        List<Building> geneBanks = ___pawn.Map.listerBuildings.AllBuildingsColonistOfDef(OAGene_RatkinDefOf.OAGene_OAGeneBank);
        foreach (Building bank in geneBanks)
        {
            if (!ReachableForTrade(___pawn, bank))
                continue;

            CompGenepackContainer compGenepackContainer = bank.TryGetComp<CompGenepackContainer>();
            if (compGenepackContainer is null)
                continue;

            List<Genepack> containedGenepacks = compGenepackContainer.ContainedGenepacks;
            foreach (Genepack pack in containedGenepacks)
                yield return pack;
        }
    }

    private static bool ReachableForTrade(Pawn pawn, Thing thing)
    {
        Thing partThing = thing;
        if (HaulAIUtility.IsInHaulableInventory(thing))
            partThing = thing.SpawnedParentOrMe;

        if (pawn.Map != partThing.MapHeld)
            return false;

        return pawn.Map.reachability.CanReach(pawn.Position, partThing, PathEndMode.Touch, TraverseMode.PassDoors, Danger.Some);
    }

}