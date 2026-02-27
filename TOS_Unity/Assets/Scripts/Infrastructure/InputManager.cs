// ============================================================
// IInputProvider.cs — Cross-platform input abstraction
// GDD Source: 04_technical_architecture.md §C.4, §E.1
// ============================================================

using UnityEngine;

namespace TOS.Infrastructure
{
    /// <summary>
    /// Abstracts pointer input across PC (mouse) and mobile (touch).
    /// Allows seamless cross-platform orb dragging and UI interaction.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>Current pointer position in screen coordinates.</summary>
        Vector2 PointerPosition { get; }

        /// <summary>True on the frame the pointer is first pressed.</summary>
        bool PointerDown { get; }

        /// <summary>True on the frame the pointer is released.</summary>
        bool PointerUp { get; }

        /// <summary>True while the pointer is held down.</summary>
        bool PointerPressed { get; }
    }

    /// <summary>
    /// Mouse input for PC/Editor.
    /// </summary>
    public class MouseInputProvider : IInputProvider
    {
        public Vector2 PointerPosition => Input.mousePosition;
        public bool PointerDown => Input.GetMouseButtonDown(0);
        public bool PointerUp => Input.GetMouseButtonUp(0);
        public bool PointerPressed => Input.GetMouseButton(0);
    }

    /// <summary>
    /// Touch input for mobile devices.
    /// </summary>
    public class TouchInputProvider : IInputProvider
    {
        public Vector2 PointerPosition =>
            Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;

        public bool PointerDown =>
            Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;

        public bool PointerUp =>
            Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;

        public bool PointerPressed =>
            Input.touchCount > 0;
    }

    /// <summary>
    /// Singleton input manager that auto-detects platform and provides
    /// unified pointer access. Attach to a persistent GameObject.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private IInputProvider _provider;

        public Vector2 PointerPosition => _provider.PointerPosition;
        public bool PointerDown => _provider.PointerDown;
        public bool PointerUp => _provider.PointerUp;
        public bool PointerPressed => _provider.PointerPressed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID || UNITY_IOS
            _provider = new TouchInputProvider();
#else
            _provider = new MouseInputProvider();
#endif
        }
    }
}
