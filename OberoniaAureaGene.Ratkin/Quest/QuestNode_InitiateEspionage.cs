using JetBrains.Annotations;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Ratkin;

//初始化间谍行动（有QuestPart）
public class QuestNode_InitiateEspionage : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;

    public SlateRef<Site> site;

    [MustTranslate]
    public SlateRef<string> customLabel;

    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }
    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        QuestPart_InitiateEspionage questPart_InitiateEspionage = new()
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"),
            site = site.GetValue(slate),
            customLabel = customLabel.GetValue(slate)
        };
        QuestGen.quest.AddPart(questPart_InitiateEspionage);
    }
}