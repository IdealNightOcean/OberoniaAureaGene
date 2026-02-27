using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[DefOf]
public static class Snowstorm_MiscDefOf
{
    /// <summary>
    /// 冰晶刺伤
    /// </summary>
    public static DamageDef OAGene_IceStab;

    /// <summary>
    /// 星月夜特效
    /// </summary>
    public static FleckDef OAGene_StarryGlow;

    public static IsolatedPawnGroupMakerDef OAGene_GroupMaker_SnowstormCultist;

    /// <summary>
    /// 气温骤降
    /// </summary>
    public static GameConditionDef OAGene_SnowstormPrecursor;
    /// <summary>
    /// 风晶雪树的降温
    /// </summary>
    public static GameConditionDef OAGene_SnowyCrystalTreeCooler;
    /// <summary>
    /// 终局风雪
    /// </summary>
    public static GameConditionDef OAGene_EndGame_ExtremeSnowstorm;

    public static JobDef OAGene_Job_TakeIceCrystalOutOfCollector;

    /// <summary>
    /// 主角状态：陷入回忆
    /// </summary>
    public static MentalBreakDef OAGene_LostInMemory;

    /// <summary>
    /// 星月夜BGM
    /// </summary>
    public static MusicTransitionDef OAGene_Transition_StarryNight;
    /// <summary>
    /// 终局风雪BGM
    /// </summary>
    public static MusicTransitionDef OAGene_Transition_Liebestraum;

    /// <summary>
    /// 终局任务：长路归乡
    /// </summary>
    public static QuestScriptDef OAGene_EndGame_Homecoming;

    /// <summary>
    /// 暴风雪破墙袭击
    /// </summary>
    public static RaidStrategyDef OAGene_SnowstormImmediateAttackBreaching;
    /// <summary>
    /// 教徒死战不退袭击
    /// </summary>
    public static RaidStrategyDef OAGene_ImmediateAttack_SnowstormCultist;

    /// <summary>
    /// 终局BGM
    /// </summary>
    public static SongDef OAGene_IGiorni;

    public static TraderKindDef OAGene_Trader_SnowstormCamp;

    /// <summary>
    /// 极端暴风雪
    /// </summary>
    public static WeatherDef OAGene_SnowExtreme;
    /// <summary>
    /// 冰晶暴风雪
    /// </summary>
    public static WeatherDef OAGene_IceSnowExtreme;
    /// <summary>
    /// 冰晶雨
    /// </summary>
    public static WeatherDef OAGene_IceRain;

    /// <summary>
    /// 家乡
    /// </summary>
    public static WorldObjectDef OAGene_Hometown;
    /// <summary>
    /// 家乡（封存状态）
    /// </summary>
    public static WorldObjectDef OAGene_Hometown_Sealed;
    static Snowstorm_MiscDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_MiscDefOf));
    }
}

[DefOf]
public static class Snowstorm_ThingDefOf
{
    /// <summary>
    /// 风雪碎晶
    /// </summary>
    public static ThingDef OAGene_IceCrystal;
    /// <summary>
    /// 风雪碎晶收集器
    /// </summary>
    public static ThingDef OAGene_IceCrystalCollector;
    /// <summary>
    /// 碎晶花
    /// </summary>
    public static ThingDef OAGene_Plant_IceCrystalFlower;
    /// <summary>
    /// 风雪火把
    /// </summary>
    public static ThingDef OAGene_AntiSnowTorch;
    /// <summary>
    /// 风雪树种
    /// </summary>
    public static ThingDef OAGene_Plant_SnowyCrystalTree_Seed;
    static Snowstorm_ThingDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_ThingDefOf));
    }
}


[DefOf]
public static class Snowstorm_HediffDefOf
{
    /// <summary>
    /// 主角归乡健康状态（用于心情）
    /// </summary>
    public static HediffDef OAGene_Hediff_ProtagonistHomecoming;
    /// <summary>
    /// 主角回忆健康状态（用于机制)
    /// </summary>
    public static HediffDef OAGene_Hediff_ProtagonistHomecomed;

    /// <summary>
    /// 充足御寒准备（敌人）
    /// </summary>
    public static HediffDef OAGene_Hediff_ColdPreparation_Enemy;
    /// <summary>
    /// 充足御寒准备（中立）
    /// </summary>
    public static HediffDef OAGene_Hediff_ColdPreparation_Neutral;
    /// <summary>
    /// 求生的希望
    /// </summary>
    public static HediffDef OAGene_Hediff_HopeForSurvival;
    /// <summary>
    /// 经历暴风雪
    /// </summary>
    public static HediffDef OAGene_Hediff_ExperienceSnowstorm;
    /// <summary>
    /// 隐匿于风雪
    /// </summary>
    public static HediffDef OAGene_Hediff_HideInSnowstorm;

    /// <summary>
    /// 忘我
    /// </summary>
    public static HediffDef OAGene_Hediff_SnowstormOblivious;
    /// <summary>
    /// 不理想的愤怒
    /// </summary>
    public static HediffDef OAGene_Hediff_SnowstormAngry;

    /// <summary>
    /// 风雪教徒健康状态
    /// </summary>
    public static HediffDef OAGene_Hediff_SnowstormCultist;
    /// <summary>
    /// 难民健康状态（用于心情）
    /// </summary>
    public static HediffDef OAGene_Hediff_SnowstormStrugglers;
    /// <summary>
    /// 特殊敲击兽
    /// </summary>
    public static HediffDef OAGene_Hediff_SpecialThrumbo;

    /// <summary>
    /// 冰晶花海
    /// </summary>
    public static HediffDef OAGene_Hediff_IceCrystalFlowerSea;
    static Snowstorm_HediffDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_HediffDefOf));
    }
}


[DefOf]
public static class Snowstorm_ThoughtDefOf
{
    /// <summary>
    /// 主角归乡心情
    /// </summary>
    public static ThoughtDef OAGene_Thought_ProtagonistHomecoming;

    /// <summary>
    /// 暴风雪结束心情
    /// </summary>
    public static ThoughtDef OAGene_Thought_SnowstormEnd;
    /// <summary>
    /// 星月夜主角心情
    /// </summary>
    public static ThoughtDef OAGene_Thought_StarryNightP;
    /// <summary>
    /// 难民心情
    /// </summary>
    public static ThoughtDef OAGene_Thought_SnowstormStrugglers;

    /// <summary>
    /// 难民心情
    /// </summary>
    public static ThoughtDef OAGene_Thought_SnowstormCultistConvert;

    /// <summary>
    /// 冰晶花海心情
    /// </summary>
    public static ThoughtDef OAGene_Thought_IceCrystalFlowerSea;
    static Snowstorm_ThoughtDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_ThoughtDefOf));
    }
}


[DefOf]
public static class Snowstorm_IncidentDefOf
{
    /// <summary>
    /// 终局风雪
    /// </summary>
    public static IncidentDef OAGene_EndGame_ExtremeSnowstorm;

    /// <summary>
    /// 极端暴风雪
    /// </summary>
    public static IncidentDef OAGene_ExtremeSnowstorm;
    /// <summary>
    /// 星月夜
    /// </summary>
    public static IncidentDef OAGene_StarryNight;
    /// <summary>
    /// 雪雾弥漫
    /// </summary>
    public static IncidentDef OAGene_SnowstormFog;

    /// <summary>
    /// 冰晶暴风雪
    /// </summary>
    public static IncidentDef OAGene_ExtremeIceStorm;
    /// <summary>
    /// 暴风雪的暖和
    /// </summary>
    public static IncidentDef OAGene_SnowstormWarm;
    /// <summary>
    /// 暴风雪的骤冷
    /// </summary>
    public static IncidentDef OAGene_SnowstormCold;

    /// <summary>
    /// 暴风雪破墙袭击
    /// </summary>
    public static IncidentDef OAGene_SnowstormMaliceRaid;
    /// <summary>
    /// 暴风雪破墙袭击（加强）
    /// </summary>
    public static IncidentDef OAGene_SnowstormMaliceRaid_Reinforce;
    /// <summary>
    /// 暴风雪破墙袭击（困难）
    /// </summary>
    public static IncidentDef OAGene_SnowstormMaliceRaid_Hard;
    /// <summary>
    /// 暴风雪中的恶意（袭击）
    /// </summary>
    public static IncidentDef OAGene_SnowstormRaidSource;
    [MayRequireRoyalty]
    /// <summary>
    /// 暴风雪中的恶意（气候）
    /// </summary>
    public static IncidentDef OAGene_SnowstormClimateAdjuster;
    /// <summary>
    /// 暴风雪狂热教徒袭击
    /// </summary>
    public static IncidentDef OAGene_SnowstormCultistRaid;

    /// <summary>
    /// 暴风雪中的挣扎者
    /// </summary>
    public static IncidentDef OAGene_SnowstormStrugglers;
    /// <summary>
    /// 暴风雪中的遇难商人
    /// </summary>
    public static IncidentDef OAGene_AffectedMerchant;
    /// <summary>
    /// 暴风雪中的敲击兽
    /// </summary>
    public static IncidentDef OAGene_SnowstormThrumboWanderIn;
    /// <summary>
    /// 通讯塔倒塌
    /// </summary>
    public static IncidentDef OAGene_CommunicationTowerCollapse;
    /// <summary>
    /// 动物逃离
    /// </summary>
    public static IncidentDef OAGene_SnowstormPrecursor_AnimalFlee;

    /// <summary>
    /// 暴风雪后的商队
    /// </summary>
    public static IncidentDef OAGene_AfterSnowstormTraderCaravanArrival;
    /// <summary>
    /// 风雪后的幸存者
    /// </summary>
    public static IncidentDef OAGene_SnowstormSurvivorJoins;
    static Snowstorm_IncidentDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_IncidentDefOf));
    }
}

[DefOf]
public static class Snowstorm_RimWorldDefOf
{
    /// <summary>
    /// 哈士奇
    /// </summary>
    public static PawnKindDef Husky;
    /// <summary>
    /// 立即破墙
    /// </summary>
    public static RaidStrategyDef ImmediateAttackBreaching;
    public static TerrainDef BurnedWoodPlankFloor;

    public static IncidentCategoryDef AllyAssistance;

    static Snowstorm_RimWorldDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstorm_RimWorldDefOf));
    }
}