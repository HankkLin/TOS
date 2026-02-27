// ============================================================
// SkillSO.cs — Skill data ScriptableObject
// GDD Source: 01_combat_system.md §4
// ============================================================

using UnityEngine;

namespace TOS.Characters
{
    /// <summary>
    /// ScriptableObject defining a skill's data and effects.
    /// Create instances via Assets > Create > TOS > Skill Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkill", menuName = "TOS/Skill Data")]
    public class SkillSO : ScriptableObject
    {
        [Header("Identity")]
        public string skillName;
        public string skillNameCN;
        [TextArea(3, 6)]
        public string description;
        public Core.SkillType skillType;

        [Header("Cooldown (Active/Exceed only)")]
        [Tooltip("Initial CD before first use")]
        public int initialCD = 10;
        [Tooltip("CD after skill upgrade / max level")]
        public int minCD = 5;

        [Header("Orb Cost (optional)")]
        [Tooltip("Required accumulated orb eliminations to unlock this skill")]
        public int orbCostRequired = 0;
        public Core.Element orbCostElement = Core.Element.Water;

        [Header("Awakening Cost (Exceed only)")]
        [Tooltip("Awakening gauge consumed on use")]
        public float awakeningCost = 100f;

        [Header("Effects")]
        [Tooltip("Skill effect definitions — evaluated in order")]
        public SkillEffect[] effects;

        [Header("Visuals")]
        public Sprite icon;
        public string animationTrigger; // Animator trigger name
        public GameObject vfxPrefab;    // Visual effect prefab
    }

    /// <summary>
    /// A single effect within a skill. Skills can have multiple effects.
    /// </summary>
    [System.Serializable]
    public class SkillEffect
    {
        public SkillEffectType effectType;

        [Header("Damage")]
        public float damageMultiplier = 1.0f;
        public Core.Element damageElement;
        public TargetType target = TargetType.SingleEnemy;

        [Header("Healing")]
        public float healAmount = 0f;        // flat heal
        public float healPercent = 0f;       // % of max HP

        [Header("Buff/Debuff")]
        public BuffType buffType;
        public float buffValue = 0f;
        public int buffDuration = 0;         // turns

        [Header("Orb Manipulation")]
        public OrbManipType orbManipType;
        public Core.OrbType targetOrbType;
        public Core.OrbType convertToOrbType;

        [Header("Shield")]
        public float shieldAmount = 0f;      // flat shield HP
        public float shieldPercent = 0f;     // % of caster's max HP

        [Header("Other")]
        public float extraTime = 0f;         // bonus orb drag seconds
        public float cdReduction = 0f;       // reduce party CD
    }

    public enum SkillEffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        OrbConvert,
        OrbDestroy,
        Shield,
        CDReduce,
        TimeExtend,
        Revive,
        Purify,          // Remove debuffs
        TerrainChange    // Change local terrain element
    }

    public enum TargetType
    {
        Self,
        SingleAlly,
        AllAllies,
        SingleEnemy,
        AllEnemies,
        AreaOfEffect,
        CrossPattern
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
        Poison,
        Freeze,
        Stun,
        Burn,
        Blind,
        Invincible,
        LifeSteal
    }

    public enum OrbManipType
    {
        None,
        ConvertAll,        // Convert all of one type to another
        ConvertRandom,     // Convert N random orbs
        DestroyByType,     // Destroy all of one type
        DestroyRandom,     // Destroy N random orbs
        Lock,              // Lock orbs (cannot be moved)
        Unlock,            // Remove locks
        Enhance            // Upgrade orbs to enhanced version
    }
}
