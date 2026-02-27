// ============================================================
// GameManager.cs — Bootstrap manager and persistent game state
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;

namespace TOS.Core
{
    /// <summary>
    /// Root singleton managing game lifecycle, scene transitions,
    /// and persistent state. Lives on a DontDestroyOnLoad object.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private int playerLevel = 1;
        [SerializeField] private int playerExp = 0;

        /// <summary>Current player level (affects skill unlock tiers).</summary>
        public int PlayerLevel => playerLevel;

        /// <summary>
        /// Whether passive skills are unlocked (player Lv31+).
        /// GDD: 01_combat_system.md §4.1
        /// </summary>
        public bool PassiveSkillsUnlocked => playerLevel >= 31;

        /// <summary>
        /// Whether exceed/awakening skills are unlocked (player Lv61+).
        /// GDD: 01_combat_system.md §4.1
        /// </summary>
        public bool ExceedSkillsUnlocked => playerLevel >= 61;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            Application.targetFrameRate = 60;

            Debug.Log($"[TOS] GameManager initialized — Unity {Application.unityVersion}");
            Debug.Log($"[TOS] Platform: {Application.platform}");
            Debug.Log($"[TOS] RAM: {SystemInfo.systemMemorySize}MB, GPU: {SystemInfo.graphicsDeviceName}");
        }

        /// <summary>
        /// Load a battle scene additively (keeps persistent managers).
        /// GDD: 04_technical_architecture.md §D.2
        /// </summary>
        public void LoadBattleScene(string sceneName, System.Action onComplete = null)
        {
            StartCoroutine(LoadSceneAsync(sceneName, onComplete));
        }

        /// <summary>
        /// Unload a battle scene and return to the menu layer.
        /// </summary>
        public void UnloadBattleScene(string sceneName, System.Action onComplete = null)
        {
            StartCoroutine(UnloadSceneAsync(sceneName, onComplete));
        }

        private System.Collections.IEnumerator LoadSceneAsync(
            string sceneName, System.Action onComplete)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
                sceneName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                // TODO: Update loading progress UI
                yield return null;
            }

            onComplete?.Invoke();
        }

        private System.Collections.IEnumerator UnloadSceneAsync(
            string sceneName, System.Action onComplete)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

            while (!asyncUnload.isDone)
            {
                yield return null;
            }

            onComplete?.Invoke();
        }

        /// <summary>
        /// Add experience and check for level up.
        /// </summary>
        public void AddExp(int amount)
        {
            playerExp += amount;
            // Simple exp curve: level N requires N*1000 total exp
            int requiredExp = playerLevel * 1000;
            while (playerExp >= requiredExp && playerLevel < 100)
            {
                playerExp -= requiredExp;
                playerLevel++;
                requiredExp = playerLevel * 1000;
                Debug.Log($"[TOS] Level Up! Now Lv{playerLevel}");
            }
        }
    }
}
