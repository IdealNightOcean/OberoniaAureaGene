using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(TradeUtility), "AllLaunchableThingsForTrade")]
public static class AllLaunchableThingsForTrade_Patch
{
    private static readonly HashSet<Thing> YieldedThings = [];
    [HarmonyPostfix]
    public static IEnumerable<Thing> Postfix(IEnumerable<Thing> originValue, Map map, ITrader trader = null)
    {
        foreach (Thing t in originValue)
        {
            yield return t;
        }
        YieldedThings.Clear();
        foreach (Building_OrbitalTradeBeacon item in Building_OrbitalTradeBeacon.AllPowered(map))
        {
            foreach (IntVec3 tradeableCell in item.TradeableCells)
            {
                IEnumerable<Thing> geneBanks = tradeableCell.GetThingList(map).Where(b => b.def == OAGene_RatkinDefOf.OAGene_OAGeneBank);
                foreach (Thing bank in geneBanks)
                {
                    CompGenepackContainer compGenepackContainer = bank.TryGetComp<CompGenepackContainer>();
                    if (compGenepackContainer == null)
                    {
                        continue;
                    }
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
}