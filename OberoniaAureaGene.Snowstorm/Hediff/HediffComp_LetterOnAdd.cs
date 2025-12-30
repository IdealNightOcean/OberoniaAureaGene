using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_LetterOnAdd : HediffCompProperties
{
    public LetterDef letterDef;

    [MustTranslate]
    public string letterText;

    [MustTranslate]
    public string letterLabel;

    public HediffCompProperties_LetterOnAdd()
    {
        compClass = typeof(HediffComp_LetterOnAdd);
    }
}
public class HediffComp_LetterOnAdd : HediffComp
{
    public HediffCompProperties_LetterOnAdd Props => props as HediffCompProperties_LetterOnAdd;
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);
        Find.LetterStack.ReceiveLetter(Props.letterLabel.Formatted(parent.Named("HEDIFF")), Props.letterText.Formatted(parent.pawn.Named("PAWN"), parent.Named("HEDIFF")), Props.letterDef ?? LetterDefOf.NeutralEvent, parent.pawn);
    }
}
