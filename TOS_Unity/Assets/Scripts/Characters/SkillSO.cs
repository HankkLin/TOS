// ============================================================
// SkillSO.cs — Skill data ScriptableObjects for Orb Echo system
// GDD Source: 01_combat_system.md v2.0 §3
// ============================================================

using UnityEngine;

namespace TOS.Characters
{
    // =========================================================
    // Orb Echo Skill — Character-specific chain-triggered skills
    // =========================================================

    /// <summary>
    /// Defines a character's Orb Echo skills (3-chain, 5-chain, 7+-chain).
    /// Each character has exactly ONE OrbEchoSkillSO.
    /// Create via Assets > Create > TOS > Orb Echo Skill.
    /// </summary>
    [CreateAssetMenu(fileName = "NewOrbEchoSkill", menuName = "TOS/Orb Echo Skill")]
    public class OrbEchoSkillSO : ScriptableObject
    {
        [Header("Identity")]
        public string skillSetName;
        public string skillSetNameCN;

        [Header("Basic Echo (3-Chain, 1 AP)")]
        public string basicName;
        public string basicNameCN;
        [TextArea(2, 4)]
        public string basicDescription;
        public SkillEffect[] basicEffects;
        public string basicAnimTrigger;
        public GameObject basicVFX;

        [Header("Enhanced Echo (5-Chain, 2 AP)")]
        public string enhancedName;
        public string enhancedNameCN;
        [TextArea(2, 4)]
        public string enhancedDescription;
        public SkillEffect[] enhancedEffects;
        public string enhancedAnimTrigger;
        public GameObject enhancedVFX;

        [Header("Ultimate Echo (7+-Chain, 3 AP)")]
        [TextArea(2, 4)]
        public string ultimateDescription;
        [Tooltip("If true, 7+-chain triggers BOTH basic + enhanced + extra effects")]
        public bool combinesBasicAndEnhanced = true;
        public SkillEffect[] ultimateExtraEffects; // Additional effects on top of basic+enhanced
        public GameObject ultimateVFX;
    }

    // =========================================================
    // Passive Trait — Character-specific persistent ability
    // =========================================================

    /// <summary>
    /// Defines a character's unique passive trait.
    /// Create via Assets > Create > TOS > Passive Trait.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPassiveTrait", menuName = "TOS/Passive Trait")]
    public class PassiveTraitSO : ScriptableObject
    {
        [Header("Identity")]
        public string traitName;
        public string traitNameCN;
        [TextArea(2, 4)]
        public string description;

        [Header("Condition")]
        public PassiveCondition condition;
        public float conditionThreshold; // e.g., HP% for "below X% HP"

        [Header("Effect")]
        public SkillEffect[] effects;

        [Header("Stacking")]
        [Tooltip("For race/element counting passives")]
        public PassiveScalingType scalingType;
        public float scalingValuePerStack; // e.g., +15% per unique race
    }

    public enum PassiveCondition
    {
        Always,             // Always active
        HPBelow,            // HP below threshold %
        HPAbove,            // HP above threshold %
        UniqueRaceCount,    // Scales with unique races in party
        AllyElementCount,   // Scales with same-element allies
        ComboAbove,         // When combo exceeds threshold
        TurnNumber,         // Every N turns
        EnemyDebuffed       // When enemy has status effect
    }

    public enum PassiveScalingType
    {
        None,               // Flat effect, no scaling
        PerUniqueRace,      // +X% per unique race (e.g., Molly's passive)
        PerSameElement,     // +X% per same-element ally
        PerMissingHP,       // +X% per 10% HP missing (e.g., Satan)
        PerAllyRace         // +X% per ally of specific race
    }

    // =========================================================
    // Divinity Burst — G-Mode ultimate skill
    // =========================================================

    /// <summary>
    /// Defines a character's Divinity Burst (G-Mode ultimate).
    /// Create via Assets > Create > TOS > Divinity Burst.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDivinityBurst", menuName = "TOS/Divinity Burst")]
    public class DivinityBurstSO : ScriptableObject
    {
        [Header("Identity")]
        public string burstName;
        public string burstNameCN;
        [TextArea(3, 6)]
        public string description;

        [Header("Cost")]
        [Tooltip("Divinity Gauge consumed (normally 100)")]
        public float gaugeCost = 100f;

        [Header("Effects")]
        public SkillEffect[] effects;

        [Header("Self-Cost (optional)")]
        [Tooltip("% of caster's current HP consumed on cast")]
        [Range(0f, 1f)]
        public float selfHPCostPercent = 0f;
        [Tooltip("% of entire team's current HP consumed on cast")]
        [Range(0f, 1f)]
        public float teamHPCostPercent = 0f;

        [Header("Aftermath")]
        [Tooltip("Post-burst board effect (e.g., increased heart drop)")]
        public BoardAfterEffect boardEffect;
        public int boardEffectDuration = 0; // turns

        [Header("Visuals")]
        public Sprite splashArt;
        public string animationTrigger;
        public GameObject cinematicPrefab; // Full 3D cutscene prefab
        public float cinematicDuration = 3f;
    }

    public enum BoardAfterEffect
    {
        None,
        IncreasedHeartDrop,     // +heart orb spawn rate
        IncreasedElementDrop,   // +caster element orb spawn rate
        BoardFreeze,            // Orbs don't change for N turns
        GravityInversion        // Skyfall direction flips
    }

    // =========================================================
    // Shared Skill Effect structures
    // =========================================================

    /// <summary>
    /// A single effect within a skill. Skills can have multiple effects.
    /// Used by OrbEchoSkillSO, PassiveTraitSO, and DivinityBurstSO.
    /// </summary>
    [System.Serializable]
    public class SkillEffect
    {
        public SkillEffectType effectType;

        [Header("Damage")]
        public float damageMultiplier = 1.0f;
        public Core.Element damageElement;
        public TargetType target = TargetType.SingleEnemy;
        public int hitCount = 1; // Number of hits (e.g., Molly 3-chain = 3 hits)

        [Header("Healing")]
        public float healAmount = 0f;
        public float healPercent = 0f; // % of max HP

        [Header("Buff/Debuff")]
        public BuffType buffType;
        public float buffValue = 0f;
        public int buffDuration = 0;

        [Header("Conditional")]
        [Tooltip("Extra effect when target has specific debuff")]
        public BuffType requiredDebuff;
        public float conditionalBonusMult = 1.0f; // e.g., 1.5 = crit if condition met

        [Header("Shield")]
        public float shieldAmount = 0f;
        public float shieldPercent = 0f;

        [Header("Self-Cost")]
        [Range(0f, 1f)]
        public float selfHPCostPercent = 0f; // e.g., Satan's 5-chain = 0.2

        [Header("Scaling")]
        [Tooltip("Damage scales inversely with caster HP (lower HP = more damage)")]
        public bool scaleDamageWithMissingHP = false;
        public float maxScalingMultiplier = 2.0f; // Cap at 200%
    }

    public enum SkillEffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        Shield,
        Freeze,         // 100% freeze non-boss units
        Stun,
        Slow,
        Poison,
        Undying,        // Cannot die for N turns
        Purify,         // Remove all debuffs
        TrueDamage,     // Ignores DEF
        SelfSacrifice   // Sacrifice HP for damage
    }

    public enum TargetType
    {
        Self,
        SingleAlly,
        AllAllies,
        SingleEnemy,
        AllEnemies,
        Row,            // Horizontal line
        Column,         // Vertical line
        CircleAOE,      // Around caster
        CrossAOE,       // Cross pattern from target
        LargeCircleAOE  // Wider AOE
    }

    public enum BuffType
    {
        None,
        ATKUp,
        ATKDown,
        DEFUp,
        DEFDown,
        SPDUp,
        SPDDown,
        CritRateUp,
        DamageReduction,
        Slow,
        Freeze,
        Stun,
        Poison,
        Blind,
        Invincible,
        Undying,        // HP cannot drop below 1
        ShortCircuit    // DEF = 0, cannot act (boss special)
    }
}
