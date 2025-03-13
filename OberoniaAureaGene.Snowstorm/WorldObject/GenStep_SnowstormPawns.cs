using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class GenStep_SnowstormPawns : GenStep
{
    public override int SeedPart => 228239291;
    public override void Generate(Map map, GenStepParams parms)
    {
        IntVec3 baseCenter;
        if (!MapGenerator.TryGetVar<CellRect>("RectOfInterest", out CellRect interestRect))
        {
            baseCenter = interestRect.CenterCell;
            Log.Error("No rect of interest set when running GenStep_WorkSitePawns!");
        }
        else
        {
            baseCenter = map.Center;
        }
        Faction faction = parms.sitePart.site.Faction;
        Lord singlePawnLord = LordMaker.MakeNewLord(faction, new LordJob_DefendBase(faction, baseCenter), map);
        TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassDoors);
        PawnKindDef pawnKind = ModsConfig.IdeologyActive ? PawnKindDefOf.WellEquippedTraveler : PawnKindDefOf.Villager;


        for (int i = 0; i < SitePartWorker_SnowstormCamp.EnemyCountRange.RandomInRange; i++)
        {
            Pawn pawn = PawnGenerator.GeneratePawn(pawnKind, faction);
            BaseGen.symbolStack.Push("pawn", SinglePawnResolveParams(pawn));
        }
        BaseGen.globalSettings.map = map;
        BaseGen.Generate();


        ResolveParams SinglePawnResolveParams(Pawn p)
        {
            ResolveParams resolveParams = default;
            resolveParams.rect = interestRect;
            resolveParams.faction = faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.singlePawnSpawnCellExtraPredicate = (IntVec3 x) => map.reachability.CanReachMapEdge(x, traverseParms);
            resolveParams.singlePawnToSpawn = p;
            return resolveParams;
        }

    }
}
