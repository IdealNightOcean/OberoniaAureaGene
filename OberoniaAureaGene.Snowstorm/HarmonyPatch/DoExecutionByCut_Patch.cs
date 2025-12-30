using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(ExecutionUtility), "DoExecutionByCut")]
public static class DoExecutionByCut_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(Pawn executioner, Pawn victim)
    {
        if (IsSpecialThrumbo(victim))
        {
            return !ThrumboManhunter(victim);
        }
        return true;
    }

    public static bool IsSpecialThrumbo(Pawn victim)
    {
        if (victim.def == ThingDefOf.Thrumbo && !victim.Downed && victim.health.hediffSet.HasHediff(Snowstorm_HediffDefOf.OAGene_Hediff_SpecialThrumbo))
        {
            return true;
        }
        return false;
    }

    private static bool ThrumboManhunter(Pawn victim)
    {
        Hediff hediff = victim.health.hediffSet.GetFirstHediffOfDef(Snowstorm_HediffDefOf.OAGene_Hediff_SpecialThrumbo);
        if (hediff is not null)
        {
            hediff.Severity = 2f;
        }
        victim.mindState.mentalStateHandler.Reset();
        if (victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent))
        {
            Find.TickManager.Pause();
            Find.LetterStack.ReceiveLetter("OAGene_LetterLable_SpecialThrumboManhunter".Translate(), "OAGene_Letter_SpecialThrumboManhunter".Translate(), LetterDefOf.ThreatSmall, victim);
            return true;
        }
        return false;
    }

}