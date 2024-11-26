using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_End_GetHometownMap : QuestNode
{
    [NoTranslate]
    public SlateRef<WorldObject> hometown;

    [NoTranslate]
    public SlateRef<string> storeAs;

    protected override bool TestRunInt(Slate slate)
    {
        MapParent hometown = this.hometown.GetValue(slate) as MapParent;
        if (hometown == null || hometown.Map == null)
        {
            return false;
        }
        slate.Set(storeAs.GetValue(slate), hometown.Map);
        return true;
    }
    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        MapParent hometown = this.hometown.GetValue(slate) as MapParent;
        if (hometown == null || hometown.Map == null)
        {
            return;
        }
        slate.Set(storeAs.GetValue(slate), hometown.Map);
    }

}
