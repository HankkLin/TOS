// ============================================================
// OrbBoard.cs — The 5×6 orb board data structure
// GDD: 01_combat_system.md v2.0 §2.2
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace TOS.OrbBoard
{
    /// <summary>
    /// Pure data model for the 5×6 orb board.
    /// No MonoBehaviour — can be unit tested independently.
    /// </summary>
    public class OrbBoard
    {
        public int Rows => Core.GameConstants.BOARD_ROWS;
        public int Cols => Core.GameConstants.BOARD_COLS;

        private OrbCell[,] _grid;

        /// <summary>Read-only access to a cell.</summary>
        public OrbCell this[int row, int col] => _grid[row, col];

        /// <summary>Whether skyfall drops from top (normal) or bottom (inverted).</summary>
        public bool GravityInverted { get; set; }

        public OrbBoard()
        {
            _grid = new OrbCell[Rows, Cols];
            GravityInverted = false;
        }

        // =====================================================
        // Initialization
        // =====================================================

        /// <summary>
        /// Fill the entire board with random orbs.
        /// Optionally bias toward a specific element (terrain resonance).
        /// </summary>
        public void RandomFill(Core.OrbType? biasType = null, float biasBoost = 0f)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    _grid[r, c] = new OrbCell(r, c, GetRandomOrbType(biasType, biasBoost));
                }
            }
        }

        /// <summary>
        /// Get a random standard orb type (Water/Fire/Wood/Light/Dark/Heart).
        /// With optional bias for terrain element resonance.
        /// </summary>
        public static Core.OrbType GetRandomOrbType(Core.OrbType? bias = null, float biasBoost = 0f)
        {
            Core.OrbType[] standardTypes = {
                Core.OrbType.Water, Core.OrbType.Fire, Core.OrbType.Wood,
                Core.OrbType.Light, Core.OrbType.Dark, Core.OrbType.Heart
            };

            if (bias.HasValue && biasBoost > 0f && Random.value < biasBoost)
            {
                return bias.Value;
            }

            return standardTypes[Random.Range(0, standardTypes.Length)];
        }

        // =====================================================
        // Chain Validation
        // =====================================================

        /// <summary>
        /// Validate that a chain path is legal:
        /// 1. All cells are the same OrbType
        /// 2. Each consecutive cell is adjacent (4-directional)
        /// 3. No cell appears twice in the path
        /// 4. No cell is locked or empty
        /// </summary>
        public bool ValidateChain(List<Vector2Int> path)
        {
            if (path == null || path.Count < Core.GameConstants.CHAIN_BASIC)
                return false;

            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Core.OrbType chainType = _grid[path[0].x, path[0].y].OrbType;

            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int pos = path[i];

                // Bounds check
                if (pos.x < 0 || pos.x >= Rows || pos.y < 0 || pos.y >= Cols)
                    return false;

                OrbCell cell = _grid[pos.x, pos.y];

                // Must not be locked, empty, or already visited
                if (!cell.CanBeChained() || visited.Contains(pos))
                    return false;

                // Must be same orb type
                if (cell.OrbType != chainType)
                    return false;

                // Must be adjacent to previous cell (except first)
                if (i > 0)
                {
                    Vector2Int prev = path[i - 1];
                    OrbCell prevCell = _grid[prev.x, prev.y];
                    if (!cell.IsAdjacentTo(prevCell))
                        return false;
                }

                visited.Add(pos);
            }

            return true;
        }

        /// <summary>
        /// Classify a chain length into Basic / Enhanced / Ultimate.
        /// </summary>
        public static Core.ChainLength ClassifyChain(int length)
        {
            if (length >= Core.GameConstants.CHAIN_ULTIMATE)
                return Core.ChainLength.Ultimate;
            if (length >= Core.GameConstants.CHAIN_ENHANCED)
                return Core.ChainLength.Enhanced;
            if (length >= Core.GameConstants.CHAIN_BASIC)
                return Core.ChainLength.Basic;
            return Core.ChainLength.None;
        }

        /// <summary>
        /// Get the AP cost for a given chain length.
        /// </summary>
        public static int GetAPCost(Core.ChainLength chain)
        {
            return chain switch
            {
                Core.ChainLength.Basic => Core.GameConstants.AP_COST_BASIC,
                Core.ChainLength.Enhanced => Core.GameConstants.AP_COST_ENHANCED,
                Core.ChainLength.Ultimate => Core.GameConstants.AP_COST_ULTIMATE,
                _ => 0
            };
        }

        // =====================================================
        // Elimination
        // =====================================================

        /// <summary>
        /// Eliminate all orbs at the given positions (mark as empty).
        /// Returns the OrbType that was eliminated.
        /// </summary>
        public Core.OrbType EliminateChain(List<Vector2Int> positions)
        {
            if (positions == null || positions.Count == 0)
                return Core.OrbType.Water;

            Core.OrbType eliminatedType = _grid[positions[0].x, positions[0].y].OrbType;

            foreach (var pos in positions)
            {
                _grid[pos.x, pos.y].Clear();
            }

            return eliminatedType;
        }

        // =====================================================
        // Skyfall / Gravity
        // =====================================================

        /// <summary>
        /// Drop orbs to fill empty spaces, respecting gravity direction.
        /// Returns list of (from, to) movements for animation.
        /// </summary>
        public List<(Vector2Int from, Vector2Int to)> ApplyGravity()
        {
            var movements = new List<(Vector2Int, Vector2Int)>();

            for (int c = 0; c < Cols; c++)
            {
                if (GravityInverted)
                    ApplyGravityColumnInverted(c, movements);
                else
                    ApplyGravityColumnNormal(c, movements);
            }

            return movements;
        }

        private void ApplyGravityColumnNormal(int col, List<(Vector2Int, Vector2Int)> movements)
        {
            // Normal gravity: orbs fall DOWN (row 0 = top, row 4 = bottom)
            int writeRow = Rows - 1; // Start from bottom

            // Move existing orbs down
            for (int readRow = Rows - 1; readRow >= 0; readRow--)
            {
                if (!_grid[readRow, col].IsEmpty)
                {
                    if (readRow != writeRow)
                    {
                        movements.Add((new Vector2Int(readRow, col), new Vector2Int(writeRow, col)));
                        SwapCells(readRow, col, writeRow, col);
                    }
                    writeRow--;
                }
            }

            // Fill remaining empty cells from top with new orbs
            for (int r = writeRow; r >= 0; r--)
            {
                _grid[r, col].Fill(GetRandomOrbType());
            }
        }

        private void ApplyGravityColumnInverted(int col, List<(Vector2Int, Vector2Int)> movements)
        {
            // Inverted gravity: orbs float UP (fill from bottom)
            int writeRow = 0; // Start from top

            for (int readRow = 0; readRow < Rows; readRow++)
            {
                if (!_grid[readRow, col].IsEmpty)
                {
                    if (readRow != writeRow)
                    {
                        movements.Add((new Vector2Int(readRow, col), new Vector2Int(writeRow, col)));
                        SwapCells(readRow, col, writeRow, col);
                    }
                    writeRow++;
                }
            }

            // Fill remaining empty cells from bottom
            for (int r = writeRow; r < Rows; r++)
            {
                _grid[r, col].Fill(GetRandomOrbType());
            }
        }

        private void SwapCells(int r1, int c1, int r2, int c2)
        {
            Core.OrbType tempType = _grid[r2, c2].OrbType;
            bool tempLocked = _grid[r2, c2].IsLocked;
            int tempLockTurns = _grid[r2, c2].LockTurnsRemaining;

            _grid[r2, c2].OrbType = _grid[r1, c1].OrbType;
            _grid[r2, c2].IsEmpty = _grid[r1, c1].IsEmpty;
            _grid[r2, c2].IsLocked = _grid[r1, c1].IsLocked;
            _grid[r2, c2].LockTurnsRemaining = _grid[r1, c1].LockTurnsRemaining;

            _grid[r1, c1].OrbType = tempType;
            _grid[r1, c1].IsEmpty = true;
            _grid[r1, c1].IsLocked = tempLocked;
            _grid[r1, c1].LockTurnsRemaining = tempLockTurns;
        }

        // =====================================================
        // Cascade Detection (auto-matches after skyfall)
        // =====================================================

        /// <summary>
        /// Find all groups of 3+ adjacent same-color orbs (auto-cascade matches).
        /// These are NOT player-initiated chains — they are skyfall bonus combos.
        /// Returns list of matched groups (each group = list of positions).
        /// </summary>
        public List<List<Vector2Int>> FindCascadeMatches()
        {
            bool[,] visited = new bool[Rows, Cols];
            var allGroups = new List<List<Vector2Int>>();

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (visited[r, c] || _grid[r, c].IsEmpty)
                        continue;

                    var group = new List<Vector2Int>();
                    FloodFill(r, c, _grid[r, c].OrbType, visited, group);

                    if (group.Count >= Core.GameConstants.CHAIN_BASIC)
                    {
                        allGroups.Add(group);
                    }
                }
            }

            return allGroups;
        }

        private void FloodFill(int row, int col, Core.OrbType targetType, bool[,] visited, List<Vector2Int> group)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                return;
            if (visited[row, col])
                return;
            if (_grid[row, col].IsEmpty || _grid[row, col].OrbType != targetType)
                return;

            visited[row, col] = true;
            group.Add(new Vector2Int(row, col));

            FloodFill(row - 1, col, targetType, visited, group); // Up
            FloodFill(row + 1, col, targetType, visited, group); // Down
            FloodFill(row, col - 1, targetType, visited, group); // Left
            FloodFill(row, col + 1, targetType, visited, group); // Right
        }

        // =====================================================
        // Board Events
        // =====================================================

        /// <summary>
        /// Lock specific orbs on the board (Boss: "Time Seal" mechanic).
        /// GDD: Boss Phase 2 — pin 4 orbs for 2 turns.
        /// </summary>
        public List<Vector2Int> LockRandomOrbs(int count, int turns)
        {
            var candidates = new List<Vector2Int>();
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (!_grid[r, c].IsEmpty && !_grid[r, c].IsLocked)
                        candidates.Add(new Vector2Int(r, c));

            var locked = new List<Vector2Int>();
            for (int i = 0; i < Mathf.Min(count, candidates.Count); i++)
            {
                int idx = Random.Range(0, candidates.Count);
                Vector2Int pos = candidates[idx];
                _grid[pos.x, pos.y].Lock(turns);
                locked.Add(pos);
                candidates.RemoveAt(idx);
            }
            return locked;
        }

        /// <summary>
        /// Convert random orbs to a specific type (Boss: element contamination).
        /// </summary>
        public List<Vector2Int> ConvertRandomOrbs(int count, Core.OrbType targetType)
        {
            var candidates = new List<Vector2Int>();
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (!_grid[r, c].IsEmpty && !_grid[r, c].IsLocked && _grid[r, c].OrbType != targetType)
                        candidates.Add(new Vector2Int(r, c));

            var converted = new List<Vector2Int>();
            for (int i = 0; i < Mathf.Min(count, candidates.Count); i++)
            {
                int idx = Random.Range(0, candidates.Count);
                Vector2Int pos = candidates[idx];
                _grid[pos.x, pos.y].OrbType = targetType;
                converted.Add(pos);
                candidates.RemoveAt(idx);
            }
            return converted;
        }

        /// <summary>
        /// Tick all lock timers at end of player turn.
        /// Returns positions of orbs that were unlocked.
        /// </summary>
        public List<Vector2Int> TickAllLocks()
        {
            var unlocked = new List<Vector2Int>();
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (_grid[r, c].TickLock())
                        unlocked.Add(new Vector2Int(r, c));
            return unlocked;
        }

        // =====================================================
        // Queries
        // =====================================================

        /// <summary>
        /// Get all cells of a specific orb type that can be chained.
        /// </summary>
        public List<Vector2Int> GetChainableCellsOfType(Core.OrbType type)
        {
            var result = new List<Vector2Int>();
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (_grid[r, c].OrbType == type && _grid[r, c].CanBeChained())
                        result.Add(new Vector2Int(r, c));
            return result;
        }

        /// <summary>
        /// Count orbs of each type currently on the board.
        /// </summary>
        public Dictionary<Core.OrbType, int> CountOrbTypes()
        {
            var counts = new Dictionary<Core.OrbType, int>();
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (_grid[r, c].IsEmpty) continue;
                    var type = _grid[r, c].OrbType;
                    counts[type] = counts.ContainsKey(type) ? counts[type] + 1 : 1;
                }
            }
            return counts;
        }

        /// <summary>
        /// Find the longest possible chain of a given type starting from a position.
        /// Uses DFS. Useful for AI hints.
        /// </summary>
        public List<Vector2Int> FindLongestChainFrom(int row, int col)
        {
            if (_grid[row, col].IsEmpty || _grid[row, col].IsLocked)
                return new List<Vector2Int>();

            Core.OrbType targetType = _grid[row, col].OrbType;
            var bestChain = new List<Vector2Int>();
            var currentChain = new List<Vector2Int>();
            var visited = new HashSet<Vector2Int>();

            DFSLongestChain(row, col, targetType, visited, currentChain, ref bestChain);

            return bestChain;
        }

        private void DFSLongestChain(int row, int col, Core.OrbType targetType,
            HashSet<Vector2Int> visited, List<Vector2Int> current, ref List<Vector2Int> best)
        {
            Vector2Int pos = new Vector2Int(row, col);
            if (row < 0 || row >= Rows || col < 0 || col >= Cols) return;
            if (visited.Contains(pos)) return;
            if (_grid[row, col].IsEmpty || _grid[row, col].IsLocked) return;
            if (_grid[row, col].OrbType != targetType) return;

            visited.Add(pos);
            current.Add(pos);

            if (current.Count > best.Count)
                best = new List<Vector2Int>(current);

            DFSLongestChain(row - 1, col, targetType, visited, current, ref best);
            DFSLongestChain(row + 1, col, targetType, visited, current, ref best);
            DFSLongestChain(row, col - 1, targetType, visited, current, ref best);
            DFSLongestChain(row, col + 1, targetType, visited, current, ref best);

            current.RemoveAt(current.Count - 1);
            visited.Remove(pos);
        }

        /// <summary>
        /// Debug print the board state to console.
        /// </summary>
        public string DebugPrint()
        {
            var sb = new System.Text.StringBuilder();
            string[] symbols = { "💧", "🔥", "🌿", "⭐", "🌙", "💗", "🔒", "☠️", "❓", "💣", "✨" };

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (_grid[r, c].IsEmpty)
                        sb.Append("⬜ ");
                    else if (_grid[r, c].IsLocked)
                        sb.Append("🔒 ");
                    else
                        sb.Append(symbols[(int)_grid[r, c].OrbType] + " ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
