// ============================================================
// OrbBoardManager.cs — Main MonoBehaviour controller for the orb board
// GDD: 01_combat_system.md v2.0 §2
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOS.OrbBoard
{
    /// <summary>
    /// Main controller for the Orb Echo board system.
    /// Manages the board state machine, player input, chain processing,
    /// skyfall cascades, and emits events for the combat engine.
    /// Attach to the OrbBoard root GameObject in the scene.
    /// </summary>
    public class OrbBoardManager : MonoBehaviour
    {
        // =====================================================
        // Configuration
        // =====================================================

        [Header("Board Settings")]
        [SerializeField] private float _orbSpacing = 1.2f;
        [SerializeField] private float _skyfallSpeed = 8f;
        [SerializeField] private float _eliminationDelay = 0.15f;
        [SerializeField] private float _cascadeDelay = 0.3f;

        [Header("Visual Prefab")]
        [SerializeField] private GameObject _orbPrefab;

        // =====================================================
        // State
        // =====================================================

        public Core.BoardState CurrentState { get; private set; } = Core.BoardState.Idle;

        private OrbBoard _board;
        private OrbVisual[,] _visuals;
        private ActionPointManager _apManager;
        private ComboTracker _comboTracker;
        private DivinityGauge _divinityGauge;

        // Active chain being built by the player
        private List<Vector2Int> _activeChain = new List<Vector2Int>();
        private Core.OrbType _activeChainType;

        // =====================================================
        // Public accessors
        // =====================================================

        public OrbBoard Board => _board;
        public ActionPointManager AP => _apManager;
        public ComboTracker Combo => _comboTracker;
        public DivinityGauge Divinity => _divinityGauge;
        public int ActiveChainLength => _activeChain.Count;

        // =====================================================
        // Events — consumed by Combat Engine
        // =====================================================

        /// <summary>Fired when a player chain is completed and validated.</summary>
        public event System.Action<ChainResult> OnChainCompleted;

        /// <summary>Fired when a cascade (skyfall) match occurs.</summary>
        public event System.Action<CascadeResult> OnCascadeMatch;

        /// <summary>Fired when all chains and cascades are done for this turn.</summary>
        public event System.Action OnBoardResolutionComplete;

        /// <summary>Fired when the board state changes.</summary>
        public event System.Action<Core.BoardState> OnStateChanged;

        /// <summary>Fired when AP runs out (player turn must end).</summary>
        public event System.Action OnAPDepleted;

        // =====================================================
        // Lifecycle
        // =====================================================

        private void Awake()
        {
            _board = new OrbBoard();
            _apManager = new ActionPointManager();
            _comboTracker = new ComboTracker();
            _divinityGauge = new DivinityGauge();
        }

        /// <summary>
        /// Initialize the board for a new battle.
        /// Called by CombatManager when battle starts.
        /// </summary>
        public void InitializeForBattle(Core.OrbType? terrainBias = null, float biasBoost = 0f)
        {
            _board.RandomFill(terrainBias, biasBoost);
            _comboTracker.ResetForNewBattle();
            _divinityGauge.Reset();
            _activeChain.Clear();

            CreateVisuals();
            SetState(Core.BoardState.Idle);

            Debug.Log("[OrbBoard] Initialized for battle\n" + _board.DebugPrint());
        }

        /// <summary>
        /// Begin the player's turn — reset AP, enable input.
        /// </summary>
        public void BeginPlayerTurn()
        {
            int bonusAP = _comboTracker.BonusAPNextTurn;
            _comboTracker.ResetForNewTurn();
            _apManager.ResetForNewTurn(bonusAP);

            // Tick lock timers
            var unlocked = _board.TickAllLocks();
            foreach (var pos in unlocked)
            {
                UpdateVisual(pos.x, pos.y);
            }

            SetState(Core.BoardState.Idle);
            Debug.Log($"[OrbBoard] Player turn started — {_apManager.CurrentAP} AP available");
        }

        // =====================================================
        // Input Handling — Chain Building
        // =====================================================

        /// <summary>
        /// Player taps an orb to start a new chain. Called by input system.
        /// </summary>
        public bool TryStartChain(int row, int col)
        {
            if (CurrentState != Core.BoardState.Idle)
                return false;

            if (row < 0 || row >= _board.Rows || col < 0 || col >= _board.Cols)
                return false;

            OrbCell cell = _board[row, col];
            if (!cell.CanBeChained())
                return false;

            // Start chain
            _activeChain.Clear();
            _activeChainType = cell.OrbType;
            _activeChain.Add(new Vector2Int(row, col));
            cell.IsSelected = true;

            SetState(Core.BoardState.ChainInput);
            UpdateVisual(row, col);

            Debug.Log($"[OrbBoard] Chain started at [{row},{col}] type={_activeChainType}");
            return true;
        }

        /// <summary>
        /// Player drags over another orb to extend the chain. Called by input system.
        /// </summary>
        public bool TryExtendChain(int row, int col)
        {
            if (CurrentState != Core.BoardState.ChainInput)
                return false;

            if (row < 0 || row >= _board.Rows || col < 0 || col >= _board.Cols)
                return false;

            Vector2Int pos = new Vector2Int(row, col);

            // Don't add if already in chain
            if (_activeChain.Contains(pos))
                return false;

            OrbCell cell = _board[row, col];

            // Must be same type, not locked, not empty
            if (!cell.CanBeChained() || cell.OrbType != _activeChainType)
                return false;

            // Must be adjacent to the last cell in the chain
            Vector2Int lastPos = _activeChain[_activeChain.Count - 1];
            OrbCell lastCell = _board[lastPos.x, lastPos.y];
            if (!cell.IsAdjacentTo(lastCell))
                return false;

            // Check if we can afford the next tier
            Core.ChainLength potentialChain = OrbBoard.ClassifyChain(_activeChain.Count + 1);
            int potentialCost = OrbBoard.GetAPCost(potentialChain);
            if (!_apManager.CanAfford(potentialCost))
                return false; // Can't afford upgrading to next tier

            // Add to chain
            _activeChain.Add(pos);
            cell.IsSelected = true;
            UpdateVisual(row, col);

            Debug.Log($"[OrbBoard] Chain extended to [{row},{col}] length={_activeChain.Count}");
            return true;
        }

        /// <summary>
        /// Player releases — submit the chain if valid. Called by input system.
        /// </summary>
        public bool TrySubmitChain()
        {
            if (CurrentState != Core.BoardState.ChainInput)
                return false;

            // Validate minimum chain length
            Core.ChainLength chainClass = OrbBoard.ClassifyChain(_activeChain.Count);
            if (chainClass == Core.ChainLength.None)
            {
                // Chain too short — cancel
                CancelChain();
                return false;
            }

            // Validate the complete path
            if (!_board.ValidateChain(_activeChain))
            {
                CancelChain();
                return false;
            }

            // Spend AP
            if (!_apManager.Spend(chainClass))
            {
                CancelChain();
                return false;
            }

            // Create result
            ChainResult result = new ChainResult(_activeChainType, _activeChain);

            // Add combo
            _comboTracker.AddCombo();

            // Add divinity
            _divinityGauge.AddFromChain(result);

            Debug.Log($"[OrbBoard] Chain submitted: {result}");

            // Transition to resolving
            SetState(Core.BoardState.Resolving);

            // Fire event for combat engine
            OnChainCompleted?.Invoke(result);

            // Start elimination + skyfall coroutine
            StartCoroutine(ProcessChainElimination(result));

            return true;
        }

        /// <summary>
        /// Cancel the current chain (player lifted finger on invalid position).
        /// </summary>
        public void CancelChain()
        {
            foreach (var pos in _activeChain)
            {
                _board[pos.x, pos.y].IsSelected = false;
                UpdateVisual(pos.x, pos.y);
            }
            _activeChain.Clear();
            SetState(Core.BoardState.Idle);
        }

        // =====================================================
        // Chain Processing Pipeline
        // =====================================================

        private IEnumerator ProcessChainElimination(ChainResult result)
        {
            // 1. Eliminate the chained orbs
            SetState(Core.BoardState.Eliminating);
            _board.EliminateChain(result.Path);

            // Clear selection visual
            foreach (var pos in result.Path)
            {
                UpdateVisual(pos.x, pos.y);
            }
            _activeChain.Clear();

            yield return new WaitForSeconds(_eliminationDelay);

            // 2. Apply gravity (skyfall)
            SetState(Core.BoardState.Skyfall);
            var movements = _board.ApplyGravity();
            // TODO: Animate orb movements
            RefreshAllVisuals();

            yield return new WaitForSeconds(_cascadeDelay);

            // 3. Check for cascade matches
            yield return StartCoroutine(ProcessCascades());

            // 4. Check if player has more AP
            if (_apManager.HasAP)
            {
                SetState(Core.BoardState.Idle);
            }
            else
            {
                // No more AP — signal turn end
                Debug.Log("[OrbBoard] AP depleted — player turn ending");
                OnAPDepleted?.Invoke();
            }
        }

        private IEnumerator ProcessCascades()
        {
            SetState(Core.BoardState.ChainCheck);

            while (true)
            {
                var cascadeGroups = _board.FindCascadeMatches();
                if (cascadeGroups.Count == 0)
                    break; // No more cascades

                foreach (var group in cascadeGroups)
                {
                    // Determine type
                    Core.OrbType cascadeType = _board[group[0].x, group[0].y].OrbType;

                    // Create cascade result
                    CascadeResult cascade = new CascadeResult(cascadeType, group);
                    _comboTracker.AddCombo();
                    _divinityGauge.AddFromCascade();

                    Debug.Log($"[OrbBoard] Cascade! {cascade} (Combo #{_comboTracker.CurrentCombo})");

                    // Fire event
                    OnCascadeMatch?.Invoke(cascade);

                    // Eliminate cascade
                    _board.EliminateChain(group);
                }

                RefreshAllVisuals();
                yield return new WaitForSeconds(_eliminationDelay);

                // Apply gravity again
                _board.ApplyGravity();
                RefreshAllVisuals();
                yield return new WaitForSeconds(_cascadeDelay);
            }
        }

        // =====================================================
        // Board Events (called by Combat Engine / Boss AI)
        // =====================================================

        /// <summary>
        /// Boss ability: Lock random orbs for N turns.
        /// </summary>
        public void BossLockOrbs(int count, int turns)
        {
            var locked = _board.LockRandomOrbs(count, turns);
            foreach (var pos in locked)
                UpdateVisual(pos.x, pos.y);
            Debug.Log($"[OrbBoard] Boss locked {locked.Count} orbs for {turns} turns");
        }

        /// <summary>
        /// Boss ability: Convert random orbs to poison/bomb type.
        /// </summary>
        public void BossConvertOrbs(int count, Core.OrbType targetType)
        {
            var converted = _board.ConvertRandomOrbs(count, targetType);
            foreach (var pos in converted)
                UpdateVisual(pos.x, pos.y);
            Debug.Log($"[OrbBoard] Boss converted {converted.Count} orbs to {targetType}");
        }

        /// <summary>
        /// Environment mechanic: Invert gravity direction.
        /// GDD: Mechanical City — gravity inversion every 3 turns.
        /// </summary>
        public void InvertGravity()
        {
            _board.GravityInverted = !_board.GravityInverted;
            Debug.Log($"[OrbBoard] Gravity inverted = {_board.GravityInverted}");
        }

        /// <summary>
        /// End-of-player-turn cleanup. Called before enemy phase.
        /// </summary>
        public void EndPlayerTurn()
        {
            // Signal that all board processing is complete
            OnBoardResolutionComplete?.Invoke();
        }

        // =====================================================
        // Visual Management
        // =====================================================

        private void CreateVisuals()
        {
            // Clean up existing visuals
            if (_visuals != null)
            {
                for (int r = 0; r < _board.Rows; r++)
                    for (int c = 0; c < _board.Cols; c++)
                        if (_visuals[r, c] != null)
                            Destroy(_visuals[r, c].gameObject);
            }

            _visuals = new OrbVisual[_board.Rows, _board.Cols];

            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Cols; c++)
                {
                    Vector3 worldPos = GridToWorld(r, c);

                    if (_orbPrefab != null)
                    {
                        GameObject orbGO = Instantiate(_orbPrefab, worldPos, Quaternion.identity, transform);
                        orbGO.name = $"Orb_{r}_{c}";
                        var visual = orbGO.GetComponent<OrbVisual>();
                        if (visual == null) visual = orbGO.AddComponent<OrbVisual>();
                        visual.Setup(_board[r, c]);
                        _visuals[r, c] = visual;
                    }
                }
            }
        }

        private void UpdateVisual(int row, int col)
        {
            if (_visuals != null && _visuals[row, col] != null)
            {
                _visuals[row, col].UpdateFromCell(_board[row, col]);
            }
        }

        private void RefreshAllVisuals()
        {
            if (_visuals == null) return;
            for (int r = 0; r < _board.Rows; r++)
                for (int c = 0; c < _board.Cols; c++)
                    UpdateVisual(r, c);
        }

        /// <summary>
        /// Convert grid coordinates to world position.
        /// Board is centered at this transform's position.
        /// </summary>
        public Vector3 GridToWorld(int row, int col)
        {
            float boardWidth = (_board.Cols - 1) * _orbSpacing;
            float boardHeight = (_board.Rows - 1) * _orbSpacing;

            float x = col * _orbSpacing - boardWidth / 2f;
            float y = -row * _orbSpacing + boardHeight / 2f; // Row 0 = top
            float z = 0f;

            return transform.position + new Vector3(x, y, z);
        }

        /// <summary>
        /// Convert world position to grid coordinates.
        /// Returns (-1,-1) if out of bounds.
        /// </summary>
        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 local = worldPos - transform.position;

            float boardWidth = (_board.Cols - 1) * _orbSpacing;
            float boardHeight = (_board.Rows - 1) * _orbSpacing;

            float col = (local.x + boardWidth / 2f) / _orbSpacing;
            float row = (-local.y + boardHeight / 2f) / _orbSpacing;

            int r = Mathf.RoundToInt(row);
            int c = Mathf.RoundToInt(col);

            if (r >= 0 && r < _board.Rows && c >= 0 && c < _board.Cols)
                return new Vector2Int(r, c);
            return new Vector2Int(-1, -1);
        }

        // =====================================================
        // State Machine
        // =====================================================

        private void SetState(Core.BoardState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
