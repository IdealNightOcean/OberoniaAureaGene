using Verse;

namespace OberoniaAureaGene;

public abstract class ConditionalStatAffecterBase : ConditionalStatAffecter
{
    [MustTranslate]
    public string label;
    public override string Label => label;

}
