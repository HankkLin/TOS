// ============================================================
// OrbBoardInput.cs — Input handler for orb chain selection
// GDD: 01_combat_system.md v2.0 §2.2 (tap-and-connect)
// ============================================================

using UnityEngine;

namespace TOS.OrbBoard
{
    /// <summary>
    /// Translates player pointer input (mouse/touch) into orb chain commands.
    /// Uses raycasting to detect which orb the player is pointing at,
    /// then calls OrbBoardManager to start/extend/submit chains.
    /// Attach to the same GameObject as OrbBoardManager.
    /// </summary>
    [RequireComponent(typeof(OrbBoardManager))]
    public class OrbBoardInput : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Camera _boardCamera;
        [SerializeField] private LayerMask _orbLayerMask = -1;
        [SerializeField] private float _raycastDistance = 100f;

        private OrbBoardManager _boardManager;
        private bool _isDragging = false;
        private Vector2Int _lastGridPos = new Vector2Int(-1, -1);

        private void Awake()
        {
            _boardManager = GetComponent<OrbBoardManager>();
        }

        private void Start()
        {
            if (_boardCamera == null)
                _boardCamera = Camera.main;
        }

        private void Update()
        {
            // Only process input during Idle or ChainInput states
            if (_boardManager.CurrentState != Core.BoardState.Idle &&
                _boardManager.CurrentState != Core.BoardState.ChainInput)
            {
                return;
            }

            var input = Infrastructure.InputManager.Instance;
            if (input == null) return;

            if (input.PointerDown)
            {
                HandlePointerDown(input.PointerPosition);
            }
            else if (input.PointerPressed && _isDragging)
            {
                HandlePointerDrag(input.PointerPosition);
            }
            else if (input.PointerUp && _isDragging)
            {
                HandlePointerUp();
            }
        }

        private void HandlePointerDown(Vector2 screenPos)
        {
            Vector2Int gridPos = ScreenToGrid(screenPos);
            if (gridPos.x < 0) return;

            if (_boardManager.TryStartChain(gridPos.x, gridPos.y))
            {
                _isDragging = true;
                _lastGridPos = gridPos;
            }
        }

        private void HandlePointerDrag(Vector2 screenPos)
        {
            Vector2Int gridPos = ScreenToGrid(screenPos);
            if (gridPos.x < 0) return;

            // Only process if we moved to a new cell
            if (gridPos == _lastGridPos) return;

            _boardManager.TryExtendChain(gridPos.x, gridPos.y);
            _lastGridPos = gridPos;
        }

        private void HandlePointerUp()
        {
            _isDragging = false;
            _lastGridPos = new Vector2Int(-1, -1);

            if (_boardManager.CurrentState == Core.BoardState.ChainInput)
            {
                if (!_boardManager.TrySubmitChain())
                {
                    // Chain was too short or invalid — already cancelled inside TrySubmitChain
                }
            }
        }

        /// <summary>
        /// Convert screen position to grid coordinates via raycasting
        /// or direct world-to-grid conversion.
        /// </summary>
        private Vector2Int ScreenToGrid(Vector2 screenPos)
        {
            if (_boardCamera == null) return new Vector2Int(-1, -1);

            // Option 1: Raycast against orb colliders
            Ray ray = _boardCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _orbLayerMask))
            {
                return _boardManager.WorldToGrid(hit.point);
            }

            // Option 2: Project onto board plane (fallback if no colliders)
            Plane boardPlane = new Plane(
                _boardManager.transform.forward,
                _boardManager.transform.position);

            if (boardPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                return _boardManager.WorldToGrid(worldPoint);
            }

            return new Vector2Int(-1, -1);
        }
    }
}
