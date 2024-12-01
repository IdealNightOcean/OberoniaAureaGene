using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_GameCondition : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;

    public SlateRef<GameConditionDef> gameCondition;

    public SlateRef<int> duration;

    [NoTranslate]
    public SlateRef<string> storeGameConditionDescriptionFutureAs;

    protected override bool TestRunInt(Slate slate)
    {
        Map hometownMap = slate.Get<Map>("hometownMap");
        return hometownMap != null;
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        Map hometownMap = slate.Get<Map>("hometownMap");
        if (hometownMap == null)
        {
            return;
        }
        GameCondition gameCondition = GameConditionMaker.MakeCondition(this.gameCondition.GetValue(slate), duration.GetValue(slate));
        QuestPart_GameCondition questPart_GameCondition = new()
        {
            gameCondition = gameCondition,
            mapParent = hometownMap.Parent,
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal")
        };
        QuestGen.quest.AddPart(questPart_GameCondition);
        if (!storeGameConditionDescriptionFutureAs.GetValue(slate).NullOrEmpty())
        {
            slate.Set(storeGameConditionDescriptionFutureAs.GetValue(slate), gameCondition.def.descriptionFuture);
        }
    }
}
