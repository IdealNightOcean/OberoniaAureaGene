using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[DefOf]
public static class Snowstorm_MiscDefOf
{
    public static DamageDef OAGene_IceStab; //冰晶刺伤

    public static FleckDef OAGene_StarryGlow; //星月夜特效

    public static IsolatedPawnGroupMakerDef OAGene_GroupMaker_SnowstormCultist;

    public static GameConditionDef OAGene_SnowstormPrecursor; //气温骤降
    public static GameConditionDef OAGene_SnowyCrystalTreeCooler; //风晶雪树的降温
    public static GameConditionDef OAGene_EndGame_ExtremeSnowstorm; //终局风雪

    public static JobDef OAGene_Job_TakeIceCrystalOutOfCollector;

    public static MentalBreakDef OAGene_LostInMemory; //陷入回忆

    public static MusicTransitionDef OAGene_Transition_StarryNight; //星月夜BGM

    public static RaidStrategyDef OAGene_SnowstormImmediateAttackBreaching; //暴风雪破墙袭击
    public static RaidStrategyDef OAGene_ImmediateAttack_SnowstormCultist; //教徒死战不退袭击

    public static SongDef OAGene_IGiorni; //终局BGM

    public static TraderKindDef OAGene_Trader_SnowstormCamp;

    public static WeatherDef OAGene_SnowExtreme; //极端暴风雪
    public static WeatherDef OAGene_IceSnowExtreme; //冰晶暴风雪
    public static WeatherDef OAGene_IceRain; //冰晶雨

    public static WorldObjectDef OAGene_FixedCaravan_IceCrystalFlowerSea; //冰晶花海远行队
    public static WorldObjectDef OAGene_Hometown; //家乡（封存占格子）
    public static WorldObjectDef OAGene_Hometown_Sealed; //家乡（封存占格子）
    static Snowstorm_MiscDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_MiscDefOf));
    }
}

[DefOf]
public static class Snowstorm_ThingDefOf
{
    public static ThingDef OAGene_IceCrystal; //风雪碎晶
    public static ThingDef OAGene_IceCrystalCollector; //风雪碎晶收集器
    public static ThingDef OAGene_Plant_IceCrystalFlower; //碎晶花
    public static ThingDef OAGene_AntiSnowTorch; //风雪火把
    public static ThingDef OAGene_Plant_SnowyCrystalTree_Seed; //风雪树种
    static Snowstorm_ThingDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_ThingDefOf));
    }
}


[DefOf]
public static class Snowstorm_HediffDefOf
{
    public static HediffDef OAGene_Hediff_ProtagonistHomecoming; //主角归乡健康状态（用于心情）
    public static HediffDef OAGene_Hediff_ProtagonistHomecomed; //主角回忆健康状态（用于机制)

    public static HediffDef OAGene_Hediff_PreparationWarm; //充足御寒准备
    public static HediffDef OAGene_Hediff_HopeForSurvival; //求生的希望
    public static HediffDef OAGene_Hediff_ExperienceSnowstorm; //经历暴风雪
    public static HediffDef OAGene_Hediff_HideInSnowstorm; //隐匿于风雪

    public static HediffDef OAGene_Hediff_SnowstormAngry; //不理想的愤怒

    public static HediffDef OAGene_Hediff_SnowstormCultist; //风雪教徒健康状态
    public static HediffDef OAGene_Hediff_SnowstormStrugglers; //难民健康状态（用于心情）
    public static HediffDef OAGene_Hediff_SpecialThrumbo; //特殊敲击兽

    public static HediffDef OAGene_Hediff_IceCrystalFlowerSea; //冰晶花海
    static Snowstorm_HediffDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_HediffDefOf));
    }
}


[DefOf]
public static class Snowstorm_ThoughtDefOf
{
    public static ThoughtDef OAGene_Thought_ProtagonistHomecoming; //主角归乡心情

    public static ThoughtDef OAGene_Thought_SnowstormEnd; //暴风雪结束心情
    public static ThoughtDef OAGene_Thought_StarryNightP; //星月夜主角心情
    public static ThoughtDef OAGene_Thought_SnowstormStrugglers; //难民心情

    public static ThoughtDef OAGene_Thought_SnowstormCultistConvert; //难民心情

    public static ThoughtDef OAGene_Thought_IceCrystalFlowerSea; //冰晶花海心情
    static Snowstorm_ThoughtDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_ThoughtDefOf));
    }
}


[DefOf]
public static class Snowstorm_IncidentDefOf
{
    public static IncidentDef OAGene_EndGame_ExtremeSnowstorm; //终局风雪

    public static IncidentDef OAGene_ExtremeSnowstorm; //极端暴风雪
    public static IncidentDef OAGene_StarryNight; //星月夜
    public static IncidentDef OAGene_SnowstormFog; //雪雾弥漫

    public static IncidentDef OAGene_ExtremeIceStorm; //冰晶暴风雪
    public static IncidentDef OAGene_SnowstormWarm; //暴风雪的暖和
    public static IncidentDef OAGene_SnowstormCold; //暴风雪的骤冷

    public static IncidentDef OAGene_SnowstormMaliceRaid; //暴风雪破墙袭击
    public static IncidentDef OAGene_SnowstormMaliceRaid_Reinforce; //暴风雪破墙袭击（加强）
    public static IncidentDef OAGene_SnowstormMaliceRaid_Hard; //暴风雪破墙袭击（困难）
    public static IncidentDef OAGene_SnowstormRaidSource; //暴风雪中的恶意（袭击）
    [MayRequireRoyalty]
    public static IncidentDef OAGene_SnowstormClimateAdjuster; //暴风雪中的恶意（气候）
    public static IncidentDef OAGene_SnowstormCultistRaid; //暴风雪狂热教徒袭击

    public static IncidentDef OAGene_SnowstormStrugglers; //暴风雪中的挣扎者
    public static IncidentDef OAGene_AffectedMerchant; //暴风雪中的遇难商人
    public static IncidentDef OAGene_SnowstormThrumboWanderIn; //暴风雪中的敲击兽
    public static IncidentDef OAGene_CommunicationTowerCollapse; //通讯塔倒塌
    public static IncidentDef OAGene_SnowstormPrecursor_AnimalFlee; //动物逃离

    public static IncidentDef OAGene_AfterSnowstormTraderCaravanArrival; //暴风雪后的商队
    public static IncidentDef OAGene_SnowstormSurvivorJoins; //风雪后的幸存者
    static Snowstorm_IncidentDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_IncidentDefOf));
    }
}

[DefOf]
public static class Snowstorm_RimWorldDefOf
{
    public static PawnKindDef Husky; //哈士奇
    public static RaidStrategyDef ImmediateAttackBreaching; //立即破墙
    public static TerrainDef BurnedWoodPlankFloor;

    public static IncidentCategoryDef AllyAssistance;

    static Snowstorm_RimWorldDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_RimWorldDefOf));
    }
}