using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class ScenPart_SnowstormStory : ScenPart
{
    public override void PostGameStart()
    {
        base.PostGameStart();
        Current.Game.GetComponent<GameComponent_SnowstormStory>()?.Notify_StoryActive();
    }
    public override IEnumerable<string> GetSummaryListEntries(string tag)
    {
        yield return "OAGene_SnowstoryStoryActive".Translate();
        if (tag == "PlayerStartsWith")
        {
            yield return Snowstrom_RimWorldDefOf.Husky.label.CapitalizeFirst() + " x" + 2;
        }
    }
    public override IEnumerable<Thing> PlayerStartingThings()
    {
        Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist);

        for (int i = 0; i < 2; i++)
        {
            Pawn animal = PawnGenerator.GeneratePawn(Snowstrom_RimWorldDefOf.Husky, Faction.OfPlayer);
            //训练全满
            Pawn_TrainingTracker trainingTracker = animal.training;
            if (trainingTracker != null)
            {
                IEnumerable<TrainableDef> trainableDefs = DefDatabase<TrainableDef>.AllDefsListForReading.Where(d => trainingTracker.CanAssignToTrain(d));
                foreach (TrainableDef trainableDef in trainableDefs)
                {
                    trainingTracker.SetWantedRecursive(trainableDef, true);
                    trainingTracker.Train(trainableDef, null, complete: true);
                }
            }
            //动物命名
            if (animal.Name == null || animal.Name.Numerical)
            {
                animal.Name = PawnBioAndNameGenerator.GeneratePawnName(animal);
            }
            //添加主人和牵绊
            if (animal.training.CanAssignToTrain(TrainableDefOf.Obedience).Accepted)
            {
                Pawn pawn;
                if (TrainerValidaor(protagonist, animal))
                {
                    pawn = protagonist;
                }
                else
                {
                    pawn = (from p in Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount)
                            where TrainerValidaor(p, animal)
                            select p).RandomElementWithFallback();
                }
                if (pawn != null)
                {
                    animal.training.Train(TrainableDefOf.Obedience, null, complete: true);
                    animal.training.SetWantedRecursive(TrainableDefOf.Obedience, checkOn: true);
                    animal.playerSettings.Master = pawn;
                    if (pawn.Ideo == null || pawn.Ideo.MemberWillingToDo(new HistoryEvent(HistoryEventDefOf.Bonded, pawn.Named(HistoryEventArgsNames.Doer))))
                    {
                        pawn.relations.AddDirectRelation(PawnRelationDefOf.Bond, animal);
                    }
                }
            }
            yield return animal;
        }
    }

    private static bool TrainerValidaor(Pawn pawn, Pawn animal)
    {
        if (pawn == null)
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
