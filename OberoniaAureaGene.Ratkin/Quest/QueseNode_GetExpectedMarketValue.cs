using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class QueseNode_GetExpectedMarketValue : QuestNode
{
    [NoTranslate]
    public SlateRef<string> storeAs;

    public SlateRef<ThingDef> expectedThingDef;
    public SlateRef<int> expectedThingCount;

    protected override bool TestRunInt(Slate slate)
    {
        SetVars(slate);
        return true;
    }
    protected override void RunInt()
    {
        SetVars(QuestGen.slate);
    }

    protected void SetVars(Slate slate)
    {
        float expectedMarketValue = expectedThingDef.GetValue(slate).GetStatValueAbstract(StatDefOf.MarketValue) * expectedThingCount.GetValue(slate);
        slate.Set(storeAs.GetValue(slate), expectedMarketValue);
    }
}
