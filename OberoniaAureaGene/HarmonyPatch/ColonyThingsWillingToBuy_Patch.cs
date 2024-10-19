using HarmonyLib;
using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Pawn_TraderTracker), "ColonyThingsWillingToBuy")]
public static class ColonyThingsWillingToBuy_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref Pawn_TraderTracker __instance, out Pawn __state)
    {
        __state = ReflectionUtility.GetFieldValue<Pawn>(__instance, "pawn", null);
    }


    [HarmonyPostfix]
    public static IEnumerable<Thing> Postfix(IEnumerable<Thing> originValue, Pawn __state, Pawn playerNegotiator)
    {
        foreach (Thing t in originValue)
        {
            yield return t;
        }
        if (__state == null)
        {
            yield break;
        }
        Pawn pawn = __state;
        List<Building> geneBanks = pawn.Map.listerBuildings.AllBuildingsColonistOfDef(OAGene_MiscDefOf.OAGene_OAGeneBank);
        foreach (Building bank in geneBanks)
        {
            if (!ReachableForTrade(pawn, bank))
            {
                continue;
            }
            CompGenepackContainer compGenepackContainer = bank.TryGetComp<CompGenepackContainer>();
            if (compGenepackContainer == null)
            {
                continue;
            }
            List<Genepack> containedGenepacks = compGenepackContainer.ContainedGenepacks;
            foreach (Genepack pack in containedGenepacks)
            {
                yield return pack;
            }
        }
    }
    private static bool ReachableForTrade(Pawn pawn, Thing thing)
    {
        Thing partThing = thing;
        if (HaulAIUtility.IsInHaulableInventory(thing))
        {
            partThing = thing.SpawnedParentOrMe;
        }
        if (pawn.Map != partThing.MapHeld)
        {
            return false;
        }
        return pawn.Map.reachability.CanReach(pawn.Position, partThing, PathEndMode.Touch, TraverseMode.PassDoors, Danger.Some);
    }

}

