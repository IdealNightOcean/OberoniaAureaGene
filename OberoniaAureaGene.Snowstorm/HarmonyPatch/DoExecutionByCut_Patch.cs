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
            ThrumboManhunter(victim);
            return false;
        }
        return true;
    }

    public static bool IsSpecialThrumbo(Pawn victim)
    {
        if (victim.def == ThingDefOf.Thrumbo && victim.health.hediffSet.HasHediff(Snowstrom_HediffDefOf.OAGene_Hediff_SpecialThrumbo))
        {
            return true;
        }
        return false;
    }

    private static void ThrumboManhunter(Pawn victim)
    {
        Hediff hediff = victim.health.hediffSet.GetFirstHediffOfDef(Snowstrom_HediffDefOf.OAGene_Hediff_SpecialThrumbo);
        if (hediff != null)
        {
            hediff.Severity = 2f;
        }
        victim.mindState.mentalStateHandler.Reset();
        victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
        Find.LetterStack.ReceiveLetter("OAGene_LetterLable_SpecialThrumboManhunter".Translate(), "OAGene_Letter_SpecialThrumboManhunter".Translate(), LetterDefOf.ThreatSmall, victim);
    }

}