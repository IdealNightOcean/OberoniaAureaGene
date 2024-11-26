using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_End_GenerateSnowstormRaids : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignalEnable;

    [NoTranslate]
    public SlateRef<string> inSignalDisable;

    [NoTranslate]
    public SlateRef<int> threatStartTicks;

    public SlateRef<ThreatsGeneratorParams> parms;

    public SlateRef<Map> homeMap;

    protected override bool TestRunInt(Slate slate)
    {
        if (!Find.Storyteller.difficulty.allowViolentQuests)
        {
            return false;
        }
        return homeMap.GetValue(slate) != null;
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        Map map = homeMap.GetValue(slate);
        QuestPart_End_SnowstroemThreatsGenerator questPart_SnowstroemThreatsGenerator = new()
        {
            threatStartTicks = threatStartTicks.GetValue(slate),
            inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID(inSignalEnable.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            inSignalDisable = QuestGenUtility.HardcodedSignalWithQuestID(inSignalDisable.GetValue(slate))
        };
        ThreatsGeneratorParams value = parms.GetValue(slate);
        questPart_SnowstroemThreatsGenerator.parms = value;
        questPart_SnowstroemThreatsGenerator.mapParent = map.Parent;
        QuestGen.quest.AddPart(questPart_SnowstroemThreatsGenerator);
    }
}

