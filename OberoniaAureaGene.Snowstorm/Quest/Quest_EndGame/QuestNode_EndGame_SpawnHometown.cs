using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_SpawnHometown : QuestNode_SpawnWorldObjects
{
    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        if (worldObjects.GetValue(slate) is null)
        {
            return;
        }
        List<WorldObject> sealedHometowns = Find.WorldObjects.AllWorldObjects.Where(w => w.def == Snowstorm_MiscDefOf.OAGene_Hometown_Sealed).ToList();
        for (int i = 0; i < sealedHometowns.Count; i++)
        {
            WorldObject sh = sealedHometowns[i];
            if (sh is not null && !sh.Destroyed)
            {
                sh.Destroy();
            }
        }
        base.RunInt();
    }
}
