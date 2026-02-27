// ============================================================
// ComboTracker.cs — Per-turn combo counting and bonuses
// GDD: 01_combat_system.md v2.0 §2.4
// ============================================================

using UnityEngine;

namespace TOS.OrbBoard
{
    /// <summary>
    /// Tracks combos within a single player turn.
    /// Reset at the start of each turn.
    /// </summary>
    public class ComboTracker
    {
        /// <summary>Current combo count this turn (player chains + cascade matches).</summary>
        public int CurrentCombo { get; private set; }

        /// <summary>Cumulative combo across the entire battle (for CD acceleration).</summary>
        public int TotalComboBattle { get; private set; }

        /// <summary>Bonus AP earned for next turn (from combo tier 3).</summary>
        public int BonusAPNextTurn { get; private set; }

        public ComboTracker()
        {
            CurrentCombo = 0;
            TotalComboBattle = 0;
            BonusAPNextTurn = 0;
        }

        /// <summary>
        /// Add a combo (from player chain or cascade match).
        /// </summary>
        public void AddCombo(int count = 1)
        {
            CurrentCombo += count;
            TotalComboBattle += count;

            // Check if combo tier 3 reached → award bonus AP for next turn
            if (CurrentCombo >= Core.GameConstants.COMBO_TIER3)
            {
                BonusAPNextTurn = Core.GameConstants.COMBO_TIER3_BONUS_AP;
            }
        }

        /// <summary>
        /// Get the team damage bonus multiplier for the current combo count.
        /// Returns 0.0 for no bonus, or the additive percentage (0.1, 0.2, 0.3).
        /// </summary>
        public float GetDamageBonus()
        {
            if (CurrentCombo >= Core.GameConstants.COMBO_TIER3)
                return Core.GameConstants.COMBO_BONUS3;
            if (CurrentCombo >= Core.GameConstants.COMBO_TIER2)
                return Core.GameConstants.COMBO_BONUS2;
            if (CurrentCombo >= Core.GameConstants.COMBO_TIER1)
                return Core.GameConstants.COMBO_BONUS1;
            return 0f;
        }

        /// <summary>
        /// Get the total damage multiplier (1.0 + bonus).
        /// </summary>
        public float GetDamageMultiplier()
        {
            return 1f + GetDamageBonus();
        }

        /// <summary>
        /// Check if cumulative combos have reached CD acceleration threshold.
        /// Every 10 total combos → reduce all skill CDs by 1.
        /// Returns number of CD reductions to apply.
        /// </summary>
        public int CheckCDAcceleration()
        {
            int threshold = Core.GameConstants.CD_ACCEL_COMBO_THRESHOLD;
            int reductions = TotalComboBattle / threshold;
            // Reset counter to carry over remainder
            return reductions; // Caller should track what was already applied
        }

        /// <summary>
        /// Reset at the start of a new player turn.
        /// Bonus AP is consumed by ActionPointManager, then cleared.
        /// </summary>
        public void ResetForNewTurn()
        {
            CurrentCombo = 0;
            BonusAPNextTurn = 0;
        }

        /// <summary>
        /// Full reset for a new battle.
        /// </summary>
        public void ResetForNewBattle()
        {
            CurrentCombo = 0;
            TotalComboBattle = 0;
            BonusAPNextTurn = 0;
        }
    }
}
