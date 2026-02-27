// ============================================================
// CharacterSO.cs — Character data ScriptableObject
// GDD Source: 01_combat_system.md §4, 02_narrative_worldbuilding.md §5
// ============================================================

using UnityEngine;

namespace TOS.Characters
{
    /// <summary>
    /// ScriptableObject defining a character's base data.
    /// Create instances via Assets > Create > TOS > Character Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "TOS/Character Data")]
    public class CharacterSO : ScriptableObject
    {
        [Header("Identity")]
        public string characterName;
        public string characterNameCN;  // Chinese name
        public string characterID;      // Unique identifier
        [TextArea(3, 6)]
        public string description;

        [Header("Classification")]
        public Core.Element element;
        public Core.Rarity rarity;
        public Core.Race race;
        [Range(1, 6)]
        public int starRating = 1;
        public Core.VariantCategory variantCategory = Core.VariantCategory.Original;
        public CharacterSO baseCharacter; // null if this IS the base, otherwise reference to original

        [Header("Base Stats (Lv1)")]
        public int baseHP = 1000;
        public int baseATK = 200;
        public int baseRCV = 100;  // Recovery
        public int baseSPD = 100;  // Speed (affects action order in execution phase)

        [Header("Max Stats (Lv99)")]
        public int maxHP = 5000;
        public int maxATK = 1500;
        public int maxRCV = 500;
        public int maxSPD = 150;

        [Header("Skill References")]
        public SkillSO leaderSkill;
        public SkillSO activeSkill;
        public SkillSO passiveSkill;     // Unlocked at player Lv31
        public SkillSO exceedSkill;      // Unlocked at player Lv61

        [Header("Visuals")]
        public Sprite portrait;
        public Sprite icon;
        public GameObject modelPrefab;  // 3D model for battle
        public RuntimeAnimatorController animatorController;

        /// <summary>
        /// Interpolate stats for a given level (1-99).
        /// </summary>
        public int GetHP(int level) => Mathf.RoundToInt(Mathf.Lerp(baseHP, maxHP, (level - 1) / 98f));
        public int GetATK(int level) => Mathf.RoundToInt(Mathf.Lerp(baseATK, maxATK, (level - 1) / 98f));
        public int GetRCV(int level) => Mathf.RoundToInt(Mathf.Lerp(baseRCV, maxRCV, (level - 1) / 98f));
        public int GetSPD(int level) => Mathf.RoundToInt(Mathf.Lerp(baseSPD, maxSPD, (level - 1) / 98f));
    }
}
