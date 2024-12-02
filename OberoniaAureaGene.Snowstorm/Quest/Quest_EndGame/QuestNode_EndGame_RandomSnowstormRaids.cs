using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_RandomSnowstormRaids : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignalEnable;

    [NoTranslate]
    public SlateRef<string> inSignalDisable;

    [NoTranslate]
    public SlateRef<int> threatStartTicks;

    public SlateRef<ThreatsGeneratorParams> parms;

    public SlateRef<WorldObject> hometown;

    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        MapParent hometown = slate.Get<WorldObject>("hometown") as MapParent;
        if (hometown == null || hometown.Map == null)
        {
            return;
        }
        QuestPart_EndGame_SnowstroemThreatsGenerator questPart_SnowstroemThreatsGenerator = new()
        {
            threatStartTicks = threatStartTicks.GetValue(slate),
            inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID(inSignalEnable.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            inSignalDisable = QuestGenUtility.HardcodedSignalWithQuestID(inSignalDisable.GetValue(slate))
        };
        ThreatsGeneratorParams parms = this.parms.GetValue(slate);
        questPart_SnowstroemThreatsGenerator.parms = parms;
        questPart_SnowstroemThreatsGenerator.mapParent = hometown;
        QuestGen.quest.AddPart(questPart_SnowstroemThreatsGenerator);
    }
}

