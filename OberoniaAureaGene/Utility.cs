using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public static class OAGeneUtility
{
    public static void AdjustOrAddHediff(Pawn pawn, HediffDef hediffDef, float severity = -1, int overrideDisappearTicks = -1, BodyPartRecord part = null, DamageInfo? dinfo = null, DamageWorker.DamageResult result = null)
    {
        Hediff hediff = pawn.health.GetOrAddHediff(hediffDef, part, dinfo, result);
        if (hediff == null)
        {
            return;
        }
        if (severity > 0)
        {
            hediff.Severity = severity;
        }
        if (overrideDisappearTicks > 0)
        {
            HediffComp_Disappears comp = hediff.TryGetComp<HediffComp_Disappears>();
            if (comp != null)
            {
                comp.ticksToDisappear = overrideDisappearTicks;
            }
        }
    }
}

[StaticConstructorOnStartup]
public static class IconUtility
{
    public static readonly Texture2D InsertPawnIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/InsertPawn");
    public static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

    public static readonly Texture2D CommandContinueWorkTrue = ContentFinder<Texture2D>.Get("UI/Icons/OAGene_CommandAutoSelectTrue");
    public static readonly Texture2D CommandContinueWorkFalse = ContentFinder<Texture2D>.Get("UI/Icons/OAGene_CommandAutoSelectFalse");
}
