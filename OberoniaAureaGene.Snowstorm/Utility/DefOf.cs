using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[DefOf]
public static class Snowstrom_MiscDefOf
{
    public static DamageDef OAGene_IceStab; //冰晶刺伤

    public static FleckDef OAGene_StarryGlow; //星月夜特效

    public static IsolatedPawnGroupMakerDef OAGene_GroupMaker_SnowstormCultist;

    public static GameConditionDef OAGene_SnowstormPrecursor; //气温骤降
    public static GameConditionDef OAGene_SnowyCrystalTreeCooler; //风晶雪树的降温
    public static GameConditionDef OAGene_EndGame_ExtremeSnowstorm; //终局风雪

    public static JobDef OAGene_Job_TakeIceCrystalOutOfCollector;

    public static MusicTransitionDef OAGene_Transition_StarryNight; //星月夜BGM

    public static RaidStrategyDef OAGene_SnowstormImmediateAttackBreaching; //暴风雪破墙袭击

    public static TraderKindDef OAGene_Trader_SnowstormCamp;

    public static WeatherDef OAGene_SnowExtreme; //极端暴风雪
    public static WeatherDef OAGene_IceSnowExtreme; //冰晶暴风雪
    public static WeatherDef OAGene_IceRain; //冰晶雨

    public static WorldObjectDef OAGene_FixedCaravan_IceCrystalFlowerSea; //冰晶花海远行队
    public static WorldObjectDef OAGene_Hometown; //家乡（封存占格子）
    public static WorldObjectDef OAGene_Hometown_Sealed; //家乡（封存占格子）
    static Snowstrom_MiscDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstrom_MiscDefOf));
    }
}

[DefOf]
public static class Snowstrom_ThingDefOf
{
    public static ThingDef OAGene_IceCrystal; //风雪碎晶
    public static ThingDef OAGene_IceCrystalCollector; //风雪碎晶收集器
    public static ThingDef OAGene_Plant_IceCrystalFlower; //碎晶花
    public static ThingDef OAGene_AntiSnowTorch; //风雪火把
    public static ThingDef OAGene_Plant_SnowyCrystalTree_Seed; //风雪树种
    static Snowstrom_ThingDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstrom_ThingDefOf));
    }
}


[DefOf]
public static class Snowstrom_HediffDefOf
{
    public static HediffDef OAGene_Hediff_ProtagonistHomecoming; //主角归乡健康状态（用于心情）

    public static HediffDef OAGene_Hediff_PreparationWarm; //充足御寒准备
    public static HediffDef OAGene_Hediff_HopeForSurvival; //求生的希望
    public static HediffDef OAGene_Hediff_ExperienceSnowstorm; //经历暴风雪

    public static HediffDef OAGene_Hediff_SnowstormAngry; //不理想的愤怒

    public static HediffDef OAGene_Hediff_SnowstormCultist; //风雪教徒健康状态
    public static HediffDef OAGene_Hediff_SnowstromStrugglers; //难民健康状态（用于心情）
    public static HediffDef OAGene_Hediff_SpecialThrumbo; //特殊敲击兽
    static Snowstrom_HediffDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstrom_HediffDefOf));
    }
}


[DefOf]
public static class Snowstrom_ThoughtDefOf
{
    public static ThoughtDef OAGene_Thought_ProtagonistHomecoming; //主角归乡心情

    public static ThoughtDef OAGene_Thought_SnowstormEnd; //暴风雪结束心情
    public static ThoughtDef OAGene_Thought_StarryNightP; //星月夜主角心情
    public static ThoughtDef OAGene_Thought_SnowstromStrugglers; //难民心情

    public static ThoughtDef OAGene_Thought_SnowstormCultistConvert; //难民心情

    public static ThoughtDef OAGene_Thought_IceCrystalFlowerSea; //冰晶花海心情
    static Snowstrom_ThoughtDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstrom_ThoughtDefOf));
    }
}


[DefOf]
public static class Snowstrom_IncidentDefOf
{
    public static IncidentDef OAGene_EndGame_ExtremeSnowstorm; //终局风雪

    public static IncidentDef OAGene_ExtremeSnowstorm; //极端暴风雪
    public static IncidentDef OAGene_StarryNight; //星月夜
    public static IncidentDef OAGene_SnowstormFog; //雪雾弥漫

    public static IncidentDef OAGene_ExtremeIceStorm; //冰晶暴风雪
    public static IncidentDef OAGene_SnowstormWarm; //暴风雪的暖和
    public static IncidentDef OAGene_SnowstormCold; //暴风雪的骤冷

    public static IncidentDef OAGene_SnowstormMaliceRaid; //暴风雪破墙袭击
    public static IncidentDef OAGene_SnowstormRaidSource; //暴风雪中的恶意（袭击）
    [MayRequireRoyalty]
    public static IncidentDef OAGene_SnowstormClimateAdjuster; //暴风雪中的恶意（气候）

    public static IncidentDef OAGene_SnowstromStrugglers; //暴风雪中的挣扎者
    public static IncidentDef OAGene_AffectedMerchant; //暴风雪中的遇难商人
    public static IncidentDef OAGene_SnowstormThrumboWanderIn; //暴风雪中的敲击兽
    public static IncidentDef OAGene_CommunicationTowerCollapse; //通讯塔倒塌
    public static IncidentDef OAGene_SnowstormPrecursor_AnimalFlee; //动物逃离

    public static IncidentDef OAGene_AfterSnowstormTraderCaravanArrival; //暴风雪后的商队
    public static IncidentDef OAGene_SnowstormSurvivorJoins; //风雪后的幸存者
    static Snowstrom_IncidentDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstrom_IncidentDefOf));
    }
}

[DefOf]
public static class Snowstrom_RimWorldDefOf
{
    public static PawnKindDef Husky; //哈士奇
    public static RaidStrategyDef ImmediateAttackBreaching; //立即破墙
    public static TerrainDef BurnedWoodPlankFloor;

    static Snowstrom_RimWorldDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstrom_RimWorldDefOf));
    }
}