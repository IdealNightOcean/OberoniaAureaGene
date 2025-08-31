/*
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(PawnRenderer), "GetBodyPos", 
    [typeof(Vector3), typeof(PawnPosture), typeof(bool)],
    [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out])]
public static class TestPatch
{
    [HarmonyPrefix]
    public static bool Prefix(Pawn ___pawn, ref Vector3 __result, Vector3 drawLoc, PawnPosture posture, ref bool showBody)
    {
        Pawn pawn = ___pawn;
        if (posture == PawnPosture.Standing)
        {
            showBody = true;
            __result = drawLoc;
            return false;
        }
        Building_Bed building_Bed = pawn.CurrentBed();
        Vector3 result;
        if (building_Bed != null && pawn.RaceProps.Humanlike)
        {
            showBody = building_Bed.def.building.bed_showSleeperBody;
            AltitudeLayer altLayer = (AltitudeLayer)Mathf.Max((int)building_Bed.def.altitudeLayer, 20);
            Vector3 vector = pawn.Position.ToVector3ShiftedWithAltitude(altLayer);
            Rot4 rotation = building_Bed.Rotation;
            rotation.AsInt += 2;
            float num = pawn.Drawer.renderer.BaseHeadOffsetAt(Rot4.South).z + pawn.story.bodyType.bedOffset + building_Bed.def.building.bed_pawnDrawOffset;
            Vector3 vector2 = rotation.FacingCell.ToVector3();
            result = vector - vector2 * num;
        }
        else
        {
            Log.Message("111");
            showBody = true;
            result = drawLoc;
            Log.Message(pawn.ParentHolder is null);

            if (pawn.ParentHolder is IThingHolderWithDrawnPawn thingHolderWithDrawnPawn)
            {
                Log.Message("222");
                result.y = thingHolderWithDrawnPawn.HeldPawnDrawPos_Y;
            }
            else if (pawn.ParentHolder.ParentHolder is IThingHolderWithDrawnPawn thingHolderWithDrawnPawn2)
            {
                Log.Message("333");
                result.y = thingHolderWithDrawnPawn2.HeldPawnDrawPos_Y;
            }
            else if (!pawn.Dead && pawn.CarriedBy == null && pawn.ParentHolder.Isnt<PawnFlyer>())
            {
                Log.Message("444");
                result.y = AltitudeLayer.LayingPawn.AltitudeFor();
            }
        }
        Log.Message("555");
        showBody = pawn.mindState?.duty?.def?.drawBodyOverride ?? showBody;
        __result = result;
        return false;
    }
}
*/