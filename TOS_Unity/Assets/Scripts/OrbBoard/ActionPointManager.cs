// ============================================================
// ActionPointManager.cs — 3 AP per turn action economy
// GDD: 01_combat_system.md v2.0 §2.3
// ============================================================

namespace TOS.OrbBoard
{
    /// <summary>
    /// Manages Action Points per player turn.
    /// Base 3 AP + bonus AP from previous turn's combo tier 3.
    /// </summary>
    public class ActionPointManager
    {
        /// <summary>AP remaining this turn.</summary>
        public int CurrentAP { get; private set; }

        /// <summary>Bonus AP carried from last turn's high combo.</summary>
        public int BonusAP { get; private set; }

        /// <summary>Maximum AP this turn (base + bonus).</summary>
        public int MaxAPThisTurn { get; private set; }

        /// <summary>Whether any AP remains to make another chain.</summary>
        public bool HasAP => CurrentAP > 0;

        /// <summary>Event when AP changes.</summary>
        public event System.Action<int, int> OnAPChanged; // (current, max)

        public ActionPointManager()
        {
            CurrentAP = Core.GameConstants.AP_PER_TURN;
            BonusAP = 0;
            MaxAPThisTurn = Core.GameConstants.AP_PER_TURN;
        }

        /// <summary>
        /// Check if a chain of the given classification can be afforded.
        /// </summary>
        public bool CanAfford(Core.ChainLength chain)
        {
            return CurrentAP >= OrbBoard.GetAPCost(chain);
        }

        /// <summary>
        /// Check if a specific AP amount can be afforded.
        /// </summary>
        public bool CanAfford(int cost)
        {
            return CurrentAP >= cost;
        }

        /// <summary>
        /// Spend AP for a chain. Returns false if insufficient.
        /// </summary>
        public bool Spend(Core.ChainLength chain)
        {
            int cost = OrbBoard.GetAPCost(chain);
            return Spend(cost);
        }

        /// <summary>
        /// Spend a specific AP amount. Returns false if insufficient.
        /// </summary>
        public bool Spend(int cost)
        {
            if (CurrentAP < cost)
                return false;

            CurrentAP -= cost;
            OnAPChanged?.Invoke(CurrentAP, MaxAPThisTurn);
            return true;
        }

        /// <summary>
        /// Set bonus AP for next turn (from ComboTracker tier 3 bonus).
        /// </summary>
        public void SetBonusAP(int bonus)
        {
            BonusAP = bonus;
        }

        /// <summary>
        /// Reset AP for a new turn. Applies any bonus AP from previous turn.
        /// </summary>
        public void ResetForNewTurn(int bonusFromLastTurn = 0)
        {
            BonusAP = bonusFromLastTurn;
            MaxAPThisTurn = Core.GameConstants.AP_PER_TURN + BonusAP;
            CurrentAP = MaxAPThisTurn;
            OnAPChanged?.Invoke(CurrentAP, MaxAPThisTurn);
        }

        /// <summary>
        /// Get the maximum chain length the player can afford.
        /// </summary>
        public Core.ChainLength GetMaxAffordableChain()
        {
            if (CanAfford(Core.ChainLength.Ultimate))
                return Core.ChainLength.Ultimate;
            if (CanAfford(Core.ChainLength.Enhanced))
                return Core.ChainLength.Enhanced;
            if (CanAfford(Core.ChainLength.Basic))
                return Core.ChainLength.Basic;
            return Core.ChainLength.None;
        }
    }
}
