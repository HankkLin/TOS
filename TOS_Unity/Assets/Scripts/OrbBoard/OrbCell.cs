// ============================================================
// OrbCell.cs — Single cell on the 5×6 orb board
// GDD: 01_combat_system.md v2.0 §2.2
// ============================================================

using UnityEngine;

namespace TOS.OrbBoard
{
    /// <summary>
    /// Represents a single cell on the orb board.
    /// Contains the orb type and any status effects on this cell.
    /// </summary>
    [System.Serializable]
    public class OrbCell
    {
        public int Row { get; private set; }
        public int Col { get; private set; }
        public Core.OrbType OrbType { get; set; }

        /// <summary>Whether this orb is locked (cannot be selected for chains, but can be eliminated by cascade).</summary>
        public bool IsLocked { get; set; }

        /// <summary>Whether this cell is empty (waiting for skyfall refill).</summary>
        public bool IsEmpty { get; set; }

        /// <summary>Whether this orb is currently part of an active chain selection.</summary>
        public bool IsSelected { get; set; }

        /// <summary>Turns remaining until lock expires (0 = not locked or permanent).</summary>
        public int LockTurnsRemaining { get; set; }

        public OrbCell(int row, int col, Core.OrbType orbType)
        {
            Row = row;
            Col = col;
            OrbType = orbType;
            IsLocked = false;
            IsEmpty = false;
            IsSelected = false;
            LockTurnsRemaining = 0;
        }

        /// <summary>
        /// Returns the combat element for this orb.
        /// </summary>
        public Core.Element GetElement()
        {
            return Core.AttributeChart.OrbToElement(OrbType);
        }

        /// <summary>
        /// Check if this cell can participate in a chain connection.
        /// Must be non-empty, non-locked, and not already selected.
        /// </summary>
        public bool CanBeChained()
        {
            return !IsEmpty && !IsLocked && !IsSelected;
        }

        /// <summary>
        /// Check if this cell is adjacent to another cell (4-directional, no diagonal).
        /// </summary>
        public bool IsAdjacentTo(OrbCell other)
        {
            int rowDiff = Mathf.Abs(Row - other.Row);
            int colDiff = Mathf.Abs(Col - other.Col);
            return (rowDiff + colDiff) == 1; // Manhattan distance = 1
        }

        /// <summary>
        /// Mark this cell as empty (orb eliminated).
        /// </summary>
        public void Clear()
        {
            IsEmpty = true;
            IsSelected = false;
            OrbType = Core.OrbType.Water; // placeholder, will be replaced by skyfall
        }

        /// <summary>
        /// Fill this empty cell with a new orb.
        /// </summary>
        public void Fill(Core.OrbType newType)
        {
            OrbType = newType;
            IsEmpty = false;
            IsSelected = false;
        }

        /// <summary>
        /// Lock this orb for N turns. Locked orbs can't be chained but can be cascade-eliminated.
        /// GDD: Boss mechanic "Time Seal" — pin orbs on the board.
        /// </summary>
        public void Lock(int turns)
        {
            IsLocked = true;
            LockTurnsRemaining = turns;
        }

        /// <summary>
        /// Tick down lock timer at end of turn. Returns true if lock expired.
        /// </summary>
        public bool TickLock()
        {
            if (!IsLocked) return false;
            LockTurnsRemaining--;
            if (LockTurnsRemaining <= 0)
            {
                IsLocked = false;
                LockTurnsRemaining = 0;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            string status = IsLocked ? "🔒" : IsEmpty ? "⬜" : IsSelected ? "✓" : "";
            return $"[{Row},{Col}:{OrbType}{status}]";
        }
    }
}
