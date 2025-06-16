using RimWorld;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene.Snowstorm;

internal class MentalBreakWorker_LostInMemory : MentalBreakWorker
{
    public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
    {
        if (pawn.mindState?.mentalStateHandler is null)
        {
            return false;
        }

        bool flag = pawn.mindState.mentalStateHandler.TryStartMentalState(def.mentalState, reason, forced: true, forceWake: true, causedByMood, null, transitionSilently: true);
        if (flag)
        {
            Messages.Message("OAGene_Message_PawnLostInMemory".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.NegativeEvent);
        }
        return flag;
    }
}
