﻿using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormCultistArrival : IncidentWorker_IsolatedTraderCaravanArrival
{
    protected override IsolatedPawnGroupMakerDef PawnGroupMakerDef => Snowstrom_MiscDefOf.OAGene_GroupMaker_SnowstormCultist;

    protected override List<Pawn> SpawnTradePawns(IncidentParms parms, PawnGroupMakerParms groupMakerParms, PawnGroupMaker groupMaker)
    {
        List<Pawn> pawns = base.SpawnTradePawns(parms, groupMakerParms, groupMaker);
        foreach (Pawn pawn in pawns)
        {
            if (pawn.RaceProps.Humanlike)
            {
                pawn.health.AddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_SnowstormCultist);
            }
        }
        return pawns;
    }
    protected override void SendLetter(IncidentParms parms, List<Pawn> pawns)
    {
        TaggedString letterLabel = "OAGene_LetterLabelSnowstormCultistArrival".Translate(parms.faction.Name, parms.traderKind.label).CapitalizeFirst();
        TaggedString letterText = "OAGene_LetterSnowstormCultistArrival".Translate(parms.faction.NameColored, parms.traderKind.label).CapitalizeFirst();
        letterText += "\n\n" + "LetterCaravanArrivalCommonWarning".Translate();
        PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(pawns, ref letterLabel, ref letterText, "LetterRelatedPawnsNeutralGroup".Translate(Faction.OfPlayer.def.pawnsPlural), informEvenIfSeenBefore: true);
        SendStandardLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, parms, pawns[0]);
    }
}
