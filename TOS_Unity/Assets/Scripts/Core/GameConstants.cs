// ============================================================
// GameConstants.cs — Game-wide constants and balance values
// GDD Source: 01_combat_system.md §2-3, 03_level_environment.md
// ============================================================

namespace TOS.Core
{
    /// <summary>
    /// Central repository for all game balance constants.
    /// All values sourced from GDD with section references.
    /// </summary>
    public static class GameConstants
    {
        // === Orb Board ===
        public const int BOARD_ROWS = 5;
        public const int BOARD_COLS = 6;
        public const int TOTAL_CELLS = BOARD_ROWS * BOARD_COLS; // 30
        public const int MIN_MATCH = 3;

        // === Orb Drag Timer (seconds) — GDD §3.3 ===
        public const float DRAG_TIME_CASUAL  = -1f; // Unlimited until drag starts
        public const float DRAG_TIME_NORMAL  = 8f;
        public const float DRAG_TIME_HARD    = 6f;
        public const float DRAG_TIME_HELL    = 5f;
        public const float DRAG_TIME_NIGHTMARE = 5f;

        // === Phantom Trail Bonuses — GDD §2.2 ===
        public const int TRAIL_BONUS_TIER1 = 15;   // cells visited
        public const float TRAIL_BONUS_DMG1 = 0.20f; // +20% damage
        public const float TRAIL_AWAKEN1 = 0.15f;    // +15% awakening gauge

        public const int TRAIL_BONUS_TIER2 = 25;
        public const float TRAIL_BONUS_DMG2 = 0.40f;
        public const float TRAIL_AWAKEN2 = 0.30f;

        public const int TRAIL_BONUS_TIER3 = 30;   // full board
        public const float TRAIL_BONUS_DMG3 = 0.60f;
        public const float TRAIL_AWAKEN3 = 0.50f;

        // === Combo Multipliers — GDD §2.3 ===
        public const int COMBO_TIER1 = 5;
        public const float COMBO_MULT1 = 1.5f;

        public const int COMBO_TIER2 = 8;
        public const float COMBO_MULT2 = 2.0f;

        public const int COMBO_TIER3 = 10;
        public const float COMBO_MULT3 = 2.5f;

        public const int COMBO_EXCEED = 15;
        public const float COMBO_EXCEED_MULT = 3.0f;
        public const float COMBO_EXCEED_AWAKEN = 0.50f;
        public const float COMBO_EXCEED_EXTRA_TIME = 3.0f;

        // === Attribute Advantage — GDD §1.1 ===
        public const float ATTRIBUTE_ADVANTAGE = 1.5f;
        public const float ATTRIBUTE_DISADVANTAGE = 0.5f;
        public const float ATTRIBUTE_NEUTRAL = 1.0f;

        // === Energy Mark Levels — GDD §2.2 ===
        public const int ENERGY_MARK_MAX_LEVEL = 3;
        public const float ENERGY_MARK_LV2_CRIT = 0.15f;  // +15% crit rate

        // === Awakening Gauge ===
        public const float AWAKENING_GAUGE_MAX = 100f;

        // === Battle Timing — GDD §3.3 ===
        public const float EXECUTION_PHASE_DURATION = 3.0f;
        public const float ENEMY_PHASE_DURATION = 2.0f;
        public const float COMBO_ANIMATION_PER_HIT = 0.3f;

        // === Team ===
        public const int TEAM_SIZE = 5;

        // === Skill CD Reduction — GDD §3.1 ===
        public const int CD_REDUCE_HEART_ELIM = 1;          // Heart orb elimination → party CD-1
        public const int CD_REDUCE_ELEMENT_5PLUS = 2;       // 5+ same element → that char CD-2

        // === Terrain Attribute Modifiers — GDD §3.2, 03_level_environment.md ===
        public const float TERRAIN_ADVANTAGE_BONUS = 0.30f;    // +30%
        public const float TERRAIN_DISADVANTAGE_PENALTY = 0.20f; // -20%

        // === Height Combat — GDD 03_level_environment.md Map 1 ===
        public const float HEIGHT_ADVANTAGE_BONUS = 0.20f;     // +20% dmg from above
        public const float HEIGHT_DISADVANTAGE_PENALTY = 0.10f; // -10% dmg attacking upward
        public const float HEIGHT_THRESHOLD_METERS = 5.0f;

        // === Fragment System — GDD §5.2 ===
        public const int SSR_FRAGMENT_REQUIRED = 100;

        // === Input ===
        public const float MIN_DRAG_DISTANCE_PX = 10f;
        public const float DRAG_CONFIRM_DELAY = 0.05f; // seconds

        // === Difficulty Boss Multipliers — GDD 03_level_environment.md ===
        public static readonly float[] BOSS_HP_MULTIPLIER = { 1f, 2f, 4f, 6f };
        public static readonly float[] BOSS_ATK_MULTIPLIER = { 1f, 1.5f, 2.5f, 4f };
        public static readonly int[] FIRST_CLEAR_COMBO_REQ = { 3, 5, 7, 10 };
    }
}
