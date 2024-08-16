using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Verb_MeleeAttack), "GetDodgeChance")]
public static class GetDodgeChance_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref float __result, LocalTargetInfo target)
    {
        if (target.Thing is not Pawn pawn)
        {
            return;
        }
        bool flag = Rand.Chance(__result);
        if (flag)
        {
            __result = 2f;
            return;
        }
        if (pawn.genes != null && pawn.genes.HasActiveGene(OberoniaAureaGeneDefOf.OAGene_MeleeIntouchable))
        {
            float dodgeChance = __result > 0.5f ? 0.5f : __result;
            flag = Rand.Chance(dodgeChance);
            __result = flag ? 2f : dodgeChance;
        }
    }
}
