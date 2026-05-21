using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Pawn_TraderTracker), nameof(Pawn_TraderTracker.ColonyThingsWillingToBuy))]
public static class ColonyThingsWillingToBuy_Patch
{
    [HarmonyPostfix]
    public static IEnumerable<Thing> Postfix(IEnumerable<Thing> originValue, Pawn ___pawn, Pawn playerNegotiator)
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