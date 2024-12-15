using OberoniaAurea_Frame;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_AffectedMerchant : IncidentWorker_NeutralGroup
{
    private static readonly IntRange MarketValueRange = new(400, 500);

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        return SnowstormUtility.IsSnowExtremeWeather(map);
    }

    protected virtual LordJob_VisitColony CreateLordJob(IncidentParms parms, Pawn pawn)
    {
        RCellFinder.TryFindRandomSpotJustOutsideColony(pawn, out var result);
        return new LordJob_VisitColony(parms.faction, result);
    }
    protected override bool TryResolveParmsGeneral(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return false;
        }
        return base.TryResolveParmsGeneral(parms);
    }
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!TryResolveParms(parms))
        {
            return false;
        }
        Pawn pawn = SpawnPawn(parms);
        if (pawn == null)
        {
            return false;
        }
        LordMaker.MakeNewLord(parms.faction, CreateLordJob(parms, pawn), map, [pawn]);

        TryConvertOnePawnToSmallTrader(pawn, parms.faction, map);
        def.letterText.Formatted(pawn.Named("PAWN")).CapitalizeFirst();
        SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, pawn);
        return true;
    }

    protected override void ResolveParmsPoints(IncidentParms parms)
    {
        parms.points = 1500;
    }

    private bool TryConvertOnePawnToSmallTrader(Pawn pawn, Faction faction, Map map)
    {
        if (!pawn.DevelopmentalStage.Adult())
        {
            return false;
        }
        Lord lord = pawn.GetLord();
        pawn.mindState.wantsToTradeWithColony = true;
        PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, actAsIfSpawned: true);
        TraderKindDef traderKindDef = FactionDefOf.OutlanderCivil.visitorTraderKinds.RandomElementByWeight((TraderKindDef traderDef) => traderDef.CalculatedCommonality);
        pawn.trader.traderKind = traderKindDef;
        pawn.inventory.DestroyAll();
        PawnInventoryGenerator.GiveRandomFood(pawn);
        ThingDef thingDef = DefDatabase<ThingDef>.AllDefsListForReading.Where(d => d != ThingDefOf.WoodLog && d.IsWithinCategory(ThingCategoryDefOf.ResourcesRaw)).RandomElement();
        if (thingDef != null)
        {
            Thing item = ThingMaker.MakeThing(thingDef);
            item.stackCount = Mathf.Max(1, (int)(MarketValueRange.RandomInRange / thingDef.BaseMarketValue));
            pawn.inventory.innerContainer.TryAdd(item);
        }
        return true;
    }
    protected Pawn SpawnPawn(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        PawnGroupMaker pawnGroupMaker = parms.faction.def.pawnGroupMakers.Where(p => p.kindDef == PawnGroupKindDefOf.Trader).RandomElementWithFallback(null);
        if (pawnGroupMaker == null)
        {
            return null;
        }
        PawnGenerationRequest generationRequest = new(pawnGroupMaker.traders.RandomElementByWeight((PawnGenOption x) => x.selectionWeight).kind, parms.faction)
        {
            AllowDead = true,
            AllowDowned = true,
            CanGeneratePawnRelations = false,
            ColonistRelationChanceFactor = 0f,
            ForceAddFreeWarmLayerIfNeeded = true,
        };
        Pawn pawn = PawnGenerator.GeneratePawn(generationRequest);
        PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, actAsIfSpawned: true);
        OAFrame_PawnUtility.AdjustOrAddHediff(pawn, OAGene_RimWorldDefOf.Hypothermia, 0.5f);

        IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, map, 5);
        GenSpawn.Spawn(pawn, loc, map);
        parms.storeGeneratedNeutralPawns?.Add(pawn);
        return pawn;
    }
}
