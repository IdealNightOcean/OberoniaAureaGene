using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_GetHometownMap : QuestNode
{
    [NoTranslate]
    public SlateRef<WorldObject> hometown;

    protected override bool TestRunInt(Slate slate)
    {
        MapParent hometown = this.hometown.GetValue(slate) as MapParent;
        if (hometown == null || hometown.Map == null)
        {
            return false;
        }
        slate.Set("hometownMap", hometown.Map);
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
        slate.Set("hometownMap", hometown.Map);
    }

}
