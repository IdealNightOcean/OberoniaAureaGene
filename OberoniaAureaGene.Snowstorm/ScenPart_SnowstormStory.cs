using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class ScenPart_SnowstormStory : ScenPart
{
    public const string SnowstormStoryTag = "OAGene_SnowstormStory";
    public override void PostGameStart()
    {
        base.PostGameStart();
        Snowstorm_StoryUtility.StoryGameComp.Notify_StoryActive();
    }
    public override string Summary(Scenario scen)
    {
        string intro = SnowstormStoryTag + "_ScenPart";
        return ScenSummaryList.SummaryWithList(scen, SnowstormStoryTag, intro.Translate()) + "\n";
    }
    public override IEnumerable<string> GetSummaryListEntries(string tag)
    {
        if (tag == SnowstormStoryTag)
        {
            yield return "OAGene_SnowstoryStoryActive".Translate();
            yield return "OAGene_SnowstoryPlayerStartsWith".Translate(Snowstorm_RimWorldDefOf.Husky.label, 2);
        }
    }
    public override IEnumerable<Thing> PlayerStartingThings()
    {
        Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist);

        for (int i = 0; i < 2; i++)
        {
            Pawn animal = PawnGenerator.GeneratePawn(Snowstorm_RimWorldDefOf.Husky, Faction.OfPlayer);
            //训练全满
            Pawn_TrainingTracker trainingTracker = animal.training;
            if (trainingTracker is not null)
            {
                IEnumerable<TrainableDef> trainableDefs = DefDatabase<TrainableDef>.AllDefsListForReading.Where(d => trainingTracker.CanAssignToTrain(d));
                foreach (TrainableDef trainableDef in trainableDefs)
                {
                    trainingTracker.SetWantedRecursive(trainableDef, true);
                    trainingTracker.Train(trainableDef, null, complete: true);
                }
            }
            //动物命名
            if (animal.Name is null || animal.Name.Numerical)
            {
                animal.Name = PawnBioAndNameGenerator.GeneratePawnName(animal);
            }
            //添加主人和牵绊
            if (animal.training.CanAssignToTrain(TrainableDefOf.Obedience).Accepted)
            {
                Pawn pawn;
                if (TrainerValidator(protagonist, animal))
                {
                    pawn = protagonist;
                }
                else
                {
                    pawn = (from p in Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount)
                            where TrainerValidator(p, animal)
                            select p).RandomElementWithFallback();
                }
                if (pawn is not null)
                {
                    animal.training.Train(TrainableDefOf.Obedience, null, complete: true);
                    animal.training.SetWantedRecursive(TrainableDefOf.Obedience, checkOn: true);
                    animal.playerSettings.Master = pawn;
                    if (pawn.Ideo is null || pawn.Ideo.MemberWillingToDo(new HistoryEvent(HistoryEventDefOf.Bonded, pawn.Named(HistoryEventArgsNames.Doer))))
                    {
                        pawn.relations.AddDirectRelation(PawnRelationDefOf.Bond, animal);
                    }
                }
            }
            yield return animal;
        }
    }

    private static bool TrainerValidator(Pawn pawn, Pawn animal)
    {
        if (pawn is null)
        {
            return false;
        }
        if (!TrainableUtility.CanBeMaster(pawn, animal, checkSpawned: false))
        {
            return false;
        }
        if (pawn.story.traits.HasTrait(TraitDefOf.Psychopath))
        {
            return false;
        }
        if (pawn.Inhumanized())
        {
            return false;
        }
        return true;
    }
}
