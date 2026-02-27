// ============================================================
// OrbVisual.cs — 3D visual representation of a single orb
// ============================================================

using UnityEngine;

namespace TOS.OrbBoard
{
    /// <summary>
    /// Visual representation of an OrbCell in the 3D scene.
    /// Handles color, selection highlight, lock indicator, and animations.
    /// Attach to each orb prefab instance.
    /// </summary>
    public class OrbVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Renderer _orbRenderer;
        [SerializeField] private GameObject _selectionHighlight;
        [SerializeField] private GameObject _lockIndicator;

        [Header("Element Colors")]
        [SerializeField] private Color _waterColor  = new Color(0.2f, 0.5f, 1f);
        [SerializeField] private Color _fireColor   = new Color(1f, 0.3f, 0.15f);
        [SerializeField] private Color _woodColor   = new Color(0.2f, 0.8f, 0.3f);
        [SerializeField] private Color _lightColor  = new Color(1f, 0.95f, 0.4f);
        [SerializeField] private Color _darkColor   = new Color(0.5f, 0.2f, 0.8f);
        [SerializeField] private Color _heartColor  = new Color(1f, 0.4f, 0.6f);
        [SerializeField] private Color _poisonColor = new Color(0.4f, 0.1f, 0.4f);
        [SerializeField] private Color _emptyColor  = new Color(0.2f, 0.2f, 0.2f, 0.3f);

        private MaterialPropertyBlock _propertyBlock;
        private static readonly int ColorID = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();

            if (_orbRenderer == null)
                _orbRenderer = GetComponent<Renderer>();
        }

        /// <summary>
        /// Initial setup from an OrbCell.
        /// </summary>
        public void Setup(OrbCell cell)
        {
            UpdateFromCell(cell);
        }

        /// <summary>
        /// Update visual to match the current cell state.
        /// </summary>
        public void UpdateFromCell(OrbCell cell)
        {
            if (cell.IsEmpty)
            {
                SetColor(_emptyColor);
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            SetColor(GetOrbColor(cell.OrbType));

            // Selection highlight
            if (_selectionHighlight != null)
                _selectionHighlight.SetActive(cell.IsSelected);

            // Lock indicator
            if (_lockIndicator != null)
                _lockIndicator.SetActive(cell.IsLocked);
        }

        /// <summary>
        /// Play elimination animation (scale down + fade).
        /// </summary>
        public void PlayElimination(System.Action onComplete = null)
        {
            // Simple scale-down — in production, use DOTween
            StartCoroutine(EliminateAnim(onComplete));
        }

        private System.Collections.IEnumerator EliminateAnim(System.Action onComplete)
        {
            float duration = 0.25f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }

            gameObject.SetActive(false);
            transform.localScale = startScale;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Play skyfall drop animation to a target position.
        /// </summary>
        public void PlayDrop(Vector3 targetPos, float speed, System.Action onComplete = null)
        {
            StartCoroutine(DropAnim(targetPos, speed, onComplete));
        }

        private System.Collections.IEnumerator DropAnim(Vector3 target, float speed, System.Action onComplete)
        {
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, target, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = target;
            onComplete?.Invoke();
        }

        private void SetColor(Color color)
        {
            if (_orbRenderer == null) return;
            _orbRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorID, color);
            _orbRenderer.SetPropertyBlock(_propertyBlock);
        }

        private Color GetOrbColor(Core.OrbType type)
        {
            return type switch
            {
                Core.OrbType.Water    => _waterColor,
                Core.OrbType.Fire     => _fireColor,
                Core.OrbType.Wood     => _woodColor,
                Core.OrbType.Light    => _lightColor,
                Core.OrbType.Dark     => _darkColor,
                Core.OrbType.Heart    => _heartColor,
                Core.OrbType.Poison   => _poisonColor,
                Core.OrbType.Locked   => _darkColor,
                Core.OrbType.Enhanced => _lightColor,
                _ => Color.white
            };
        }
    }
}
