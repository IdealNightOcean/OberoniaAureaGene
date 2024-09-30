using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormSurvivorJoin : IncidentWorker_WandererJoin
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!base.CanFireNowSub(parms))
        {
            return false;
        }
        Map map = (Map)parms.target;
        return SnowstormUtility.IsSnowExtremeWeather(map);
    }
    public override Pawn GeneratePawn()
    {
        Gender? fixedGender = null;
        if (def.pawnFixedGender != 0)
        {
            fixedGender = def.pawnFixedGender;
        }
        Ideo result = null;
        if (ModsConfig.IdeologyActive && !Find.IdeoManager.IdeosListForReading.Where((Ideo i) => !Faction.OfPlayer.ideos.Has(i)).TryRandomElementByWeight((Ideo x) => IdeoUtility.IdeoChangeToWeight(null, x), out result))
        {
            Find.IdeoManager.IdeosListForReading.Where((Ideo i) => !Faction.OfPlayer.ideos.IsPrimary(i)).TryRandomElementByWeight((Ideo x) => IdeoUtility.IdeoChangeToWeight(null, x), out result);
        }
        PawnGenerationRequest request = new(def.pawnKind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, def.pawnMustBeCapableOfViolence, 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, fixedGender, null, null, null, result)
        {
            ForcedTraits = [OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor]
        };
        return PawnGenerator.GeneratePawn(request);
    }
}
