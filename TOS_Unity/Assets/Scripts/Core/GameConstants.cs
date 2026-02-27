// ============================================================
// GameConstants.cs — Game-wide constants and balance values v2.0
// GDD Source: 01_combat_system.md v2.0
// ============================================================

namespace TOS.Core
{
    /// <summary>
    /// Central repository for all game balance constants.
    /// v2.0: Redesigned for Orb Echo + Divinity Burst mechanics.
    /// </summary>
    public static class GameConstants
    {
        // === Orb Board ===
        public const int BOARD_ROWS = 5;
        public const int BOARD_COLS = 6;
        public const int TOTAL_CELLS = BOARD_ROWS * BOARD_COLS; // 30

        // === Orb Echo Chain Thresholds — GDD §2.2 ===
        public const int CHAIN_BASIC = 3;       // 3 connected → basic echo
        public const int CHAIN_ENHANCED = 5;    // 5 connected → enhanced echo
        public const int CHAIN_ULTIMATE = 7;    // 7+ connected → ultimate echo

        // === Action Points — GDD §2.3 ===
        public const int AP_PER_TURN = 3;
        public const int AP_COST_BASIC = 1;     // 3-chain costs 1 AP
        public const int AP_COST_ENHANCED = 2;  // 5-chain costs 2 AP
        public const int AP_COST_ULTIMATE = 3;  // 7+-chain costs 3 AP

        // === Combo Bonuses — GDD §2.4 ===
        public const int COMBO_TIER1 = 2;
        public const float COMBO_BONUS1 = 0.10f;  // +10% team damage

        public const int COMBO_TIER2 = 3;
        public const float COMBO_BONUS2 = 0.20f;  // +20% team damage

        public const int COMBO_TIER3 = 4;
        public const float COMBO_BONUS3 = 0.30f;  // +30% + next turn +1 AP
        public const int COMBO_TIER3_BONUS_AP = 1;

        // === Divinity Gauge — GDD §2.5 ===
        public const float DIVINITY_GAUGE_MAX = 100f;
        public const float DIVINITY_PER_BASIC_CHAIN = 5f;
        public const float DIVINITY_PER_ENHANCED_CHAIN = 15f;
        public const float DIVINITY_PER_ULTIMATE_CHAIN = 25f;
        public const float DIVINITY_PER_SKYFALL_COMBO = 3f;
        public const float DIVINITY_PER_ADVANTAGE_HIT = 5f;
        // Damage taken: +(damagePercent × 10) — calculated dynamically

        // === Attribute Advantage — GDD §1.1 (unchanged) ===
        public const float ATTRIBUTE_ADVANTAGE = 1.5f;
        public const float ATTRIBUTE_DISADVANTAGE = 0.5f;
        public const float ATTRIBUTE_NEUTRAL = 1.0f;

        // === Damage Formula — GDD §4.3 ===
        // FinalDmg = ATK × SkillMult × AttrAdv × ComboBonus × PassiveBonus × EnvMod - DEF
        public const float MIN_DAMAGE = 1f; // Minimum damage floor

        // === Skill Multiplier Ranges — GDD §4.3 ===
        public const float BASIC_ECHO_MULT_MIN = 1.0f;
        public const float BASIC_ECHO_MULT_MAX = 1.5f;
        public const float ENHANCED_ECHO_MULT_MIN = 2.5f;
        public const float ENHANCED_ECHO_MULT_MAX = 4.0f;
        public const float ULTIMATE_ECHO_MULT_MIN = 4.0f;
        public const float ULTIMATE_ECHO_MULT_MAX = 6.0f;
        public const float DIVINITY_BURST_MULT_MIN = 10.0f;
        public const float DIVINITY_BURST_MULT_MAX = 20.0f;

        // === Numerical Model — GDD §4.2 ===
        // Character ATK range: 300(N) - 2000(SSR)
        // Boss HP range: 500K - 10M
        // Strict inflation control!

        // === Team ===
        public const int TEAM_SIZE = 5;

        // === CD Acceleration — GDD §4.4 ===
        public const int CD_ACCEL_COMBO_THRESHOLD = 10;  // Every 10 combos (cumulative)
        public const int CD_ACCEL_AMOUNT = 1;             // Reduce all CDs by 1

        // === Soul Armament Tiers — GDD §4.5 ===
        public const int SOUL_ARM_TIER1_FRAGMENTS = 10;
        public const int SOUL_ARM_TIER2_FRAGMENTS = 30;
        public const int SOUL_ARM_TIER3_FRAGMENTS = 60;
        public const int SOUL_ARM_TIER4_FRAGMENTS = 100;

        // === Environment ===
        public const float TERRAIN_ELEMENT_ORB_BOOST = 0.15f;  // +15% orb spawn rate on matching terrain
        public const float HEIGHT_ADVANTAGE_BONUS = 0.20f;
        public const float HEIGHT_DISADVANTAGE_PENALTY = 0.10f;

        // === Difficulty Boss Multipliers ===
        public static readonly float[] BOSS_HP_MULTIPLIER = { 1f, 2f, 4f, 6f };
        public static readonly float[] BOSS_ATK_MULTIPLIER = { 1f, 1.5f, 2.5f, 4f };
    }
}
