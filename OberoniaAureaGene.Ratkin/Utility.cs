using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
public static class OAGene_RatkinUtility
{
    public static bool IsRatkin(this Pawn pawn)
    {
        return pawn.def == OAGene_RatkinDefOf.Ratkin || pawn.def == OAGene_RatkinDefOf.Ratkin_Su;
    }

    public static bool HasAnyThings(Caravan caravan, ThingCategoryDef thingCategoryDef, Func<Thing, bool> validator = null)
    {
        List<Thing> list = CaravanInventoryUtility.AllInventoryItems(caravan);
        for (int i = 0; i < list.Count; i++)
        {
            Thing thing = list[i];
            if (thing.def.thingCategories.Contains(thingCategoryDef) && (validator == null || validator(thing)))
            {
                return true;
            }
        }
        return false;
    }
    public static bool HasEnoughThings(Caravan caravan, ThingCategoryDef thingCategoryDef, int count, Func<Thing, bool> validator = null)
    {
        int num = 0;
        List<Thing> list = CaravanInventoryUtility.AllInventoryItems(caravan);
        for (int i = 0; i < list.Count; i++)
        {
            Thing thing = list[i];
            if (thing.def.thingCategories.Contains(thingCategoryDef) && (validator == null || validator(thing)))
            {
                num += thing.stackCount;
            }
        }
        return num >= count;
    }
}