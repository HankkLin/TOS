// ============================================================
// StageConfigSO.cs — Level/stage configuration ScriptableObject
// GDD Source: 03_level_environment.md §Technical Implementation
// ============================================================

using UnityEngine;

namespace TOS.Environment
{
    /// <summary>
    /// ScriptableObject defining a stage's configuration.
    /// Create instances via Assets > Create > TOS > Stage Config.
    /// Maps to GDD's JSON difficulty config format.
    /// </summary>
    [CreateAssetMenu(fileName = "NewStage", menuName = "TOS/Stage Config")]
    public class StageConfigSO : ScriptableObject
    {
        [Header("Identity")]
        public string stageID;
        public string stageName;
        public string stageNameCN;
        [TextArea(2, 4)]
        public string description;
        public int chapter;             // Story chapter (0=prologue)
        public Core.Difficulty difficulty;
        public int staminaCost = 20;

        [Header("Scene")]
        public string sceneName;        // Unity scene to load
        public string baseTerrain;      // Terrain template identifier

        [Header("Environment Modules")]
        public EnvironmentModule[] environmentModules;

        [Header("Orb Board Config")]
        [Range(-0.3f, 0.3f)]
        public float heartDropRateModifier = 0f;
        [Range(0, 10)]
        public int maxLockedOrbs = 0;
        [Range(0, 10)]
        public int maxPoisonOrbs = 0;
        [Range(0, 10)]
        public int maxUnknownOrbs = 0;
        [Range(3, 15)]
        public int firstClearComboRequired = 3;

        [Header("Boss Config")]
        public float bossHPMultiplier = 1f;
        public float bossATKMultiplier = 1f;
        public string[] specialMechanics;   // e.g. "combo_shield", "reverse_altar"

        [Header("Time Limit")]
        [Tooltip("0 = no time limit")]
        public float timeLimitSeconds = 0f;

        [Header("Rewards")]
        public StageReward[] rewards;

        [Header("Unlock Conditions")]
        public string[] requiredStageClears;  // Stage IDs that must be cleared first
        public int requiredPlayerLevel = 1;
    }

    [System.Serializable]
    public class EnvironmentModule
    {
        public EnvironmentModuleType type;

        [Header("Gravity")]
        public Core.GravityState gravityState = Core.GravityState.Standard;
        public float gravityChangeInterval = 0f;  // seconds, 0 = no change

        [Header("Platform Collapse")]
        public float collapseInterval = 45f;       // seconds between collapses
        public bool chainCollapse = false;
        public float warningTime = 8f;             // seconds warning before collapse

        [Header("Terrain Element")]
        public Core.Element terrainElement;
        public float elementDamageBonus = 0.30f;
        public float elementDamagePenalty = 0.20f;

        [Header("Area Rotation")]
        public float rotationInterval = 25f;
        public bool randomRotation = false;

        [Header("Erosion / DOT")]
        public float erosionDamagePercent = 0f;   // % max HP per second
        public float erosionEscalation = 0f;      // increase per 5 seconds
        public float erosionCap = 0f;             // max % per second
    }

    public enum EnvironmentModuleType
    {
        GravityAnomaly,
        PlatformCollapse,
        ElementalTerrain,
        AreaRotation,
        WeatherEffect,
        DayNightCycle,
        AbyssErosion,
        DestructibleCover,
        MovingPlatform,
        TeleportGate,
        EnvironmentalTrap
    }

    [System.Serializable]
    public class StageReward
    {
        public RewardType rewardType;
        public string itemID;           // Character ID, material ID, etc.
        public int amount = 1;
        public float dropChance = 1.0f; // 1.0 = guaranteed
    }

    public enum RewardType
    {
        Character,
        CharacterFragment,
        EvolutionMaterial,
        Gold,
        Stamina,
        Title,
        Equipment
    }
}
