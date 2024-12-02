using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_MakeGameCondition : QuestNode
{
    [NoTranslate]
    public SlateRef<string> inSignal;

    public SlateRef<GameConditionDef> gameCondition;

    public SlateRef<int> duration;

    [NoTranslate]
    public SlateRef<string> storeGameConditionDescriptionFutureAs;

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
        GameCondition gameCondition = GameConditionMaker.MakeCondition(this.gameCondition.GetValue(slate), duration.GetValue(slate));
        QuestPart_GameCondition questPart_GameCondition = new()
        {
            gameCondition = gameCondition,
            mapParent = hometown,
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal")
        };
        QuestGen.quest.AddPart(questPart_GameCondition);
        if (!storeGameConditionDescriptionFutureAs.GetValue(slate).NullOrEmpty())
        {
            slate.Set(storeGameConditionDescriptionFutureAs.GetValue(slate), gameCondition.def.descriptionFuture);
        }
    }
}
