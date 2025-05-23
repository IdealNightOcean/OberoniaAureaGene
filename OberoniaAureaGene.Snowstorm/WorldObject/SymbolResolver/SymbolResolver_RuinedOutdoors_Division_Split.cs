﻿using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class SymbolResolver_RuinedOutdoors_Division_Split : SymbolResolver_BasePart_Outdoors_Division_Split
{
    private static readonly IntRange SpaceBetweenRange = new(1, 2);
    public override void Resolve(ResolveParams rp)
    {
        bool @bool = Rand.Bool;
        bool flag;
        if (TryFindSplitPoint(@bool, rp.rect, out var splitPoint, out var spaceBetween))
        {
            flag = @bool;
        }
        else
        {
            if (!TryFindSplitPoint(!@bool, rp.rect, out splitPoint, out spaceBetween))
            {
                Log.Warning("Could not find split point.");
                return;
            }
            flag = !@bool;
        }
        TerrainDef floorDef = rp.pathwayFloorDef ?? BaseGenUtility.RandomBasicFloorDef(rp.faction);
        ResolveParams resolveParams3;
        ResolveParams resolveParams5;
        if (flag)
        {
            ResolveParams resolveParams = rp;
            resolveParams.rect = new CellRect(rp.rect.minX, rp.rect.minZ + splitPoint, rp.rect.Width, spaceBetween);
            resolveParams.floorDef = floorDef;
            resolveParams.streetHorizontal = true;
            BaseGen.symbolStack.Push("street", resolveParams);
            ResolveParams resolveParams2 = rp;
            resolveParams2.rect = new CellRect(rp.rect.minX, rp.rect.minZ, rp.rect.Width, splitPoint);
            resolveParams3 = resolveParams2;
            ResolveParams resolveParams4 = rp;
            resolveParams4.rect = new CellRect(rp.rect.minX, rp.rect.minZ + splitPoint + spaceBetween, rp.rect.Width, rp.rect.Height - splitPoint - spaceBetween);
            resolveParams5 = resolveParams4;
        }
        else
        {
            ResolveParams resolveParams6 = rp;
            resolveParams6.rect = new CellRect(rp.rect.minX + splitPoint, rp.rect.minZ, spaceBetween, rp.rect.Height);
            resolveParams6.floorDef = floorDef;
            resolveParams6.streetHorizontal = false;
            BaseGen.symbolStack.Push("street", resolveParams6);
            ResolveParams resolveParams7 = rp;
            resolveParams7.rect = new CellRect(rp.rect.minX, rp.rect.minZ, splitPoint, rp.rect.Height);
            resolveParams3 = resolveParams7;
            ResolveParams resolveParams8 = rp;
            resolveParams8.rect = new CellRect(rp.rect.minX + splitPoint + spaceBetween, rp.rect.minZ, rp.rect.Width - splitPoint - spaceBetween, rp.rect.Height);
            resolveParams5 = resolveParams8;
        }
        if (Rand.Bool)
        {
            BaseGen.symbolStack.Push("oagene_RuinedOutdoors", resolveParams3);
            BaseGen.symbolStack.Push("oagene_RuinedOutdoors", resolveParams5);
        }
        else
        {
            BaseGen.symbolStack.Push("oagene_RuinedOutdoors", resolveParams5);
            BaseGen.symbolStack.Push("oagene_RuinedOutdoors", resolveParams3);
        }
    }
    private bool TryFindSplitPoint(bool horizontal, CellRect rect, out int splitPoint, out int spaceBetween)
    {
        int num = horizontal ? rect.Height : rect.Width;
        spaceBetween = SpaceBetweenRange.RandomInRange;
        spaceBetween = Mathf.Min(spaceBetween, num - 10);
        if (spaceBetween < SpaceBetweenRange.min)
        {
            splitPoint = -1;
            return false;
        }
        splitPoint = Rand.RangeInclusive(5, num - 5 - spaceBetween);
        return true;
    }
}
