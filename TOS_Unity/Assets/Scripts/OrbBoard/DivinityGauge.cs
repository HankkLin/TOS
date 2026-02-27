// ============================================================
// DivinityGauge.cs — Shared G-meter for Divinity Burst
// GDD: 01_combat_system.md v2.0 §2.5
// ============================================================

using UnityEngine;

namespace TOS.OrbBoard
{
    /// <summary>
    /// Shared team resource that charges from 0→100 to enable
    /// Divinity Burst (G-Mode ultimate) for one character.
    /// </summary>
    public class DivinityGauge
    {
        /// <summary>Current gauge value (0-100).</summary>
        public float CurrentValue { get; private set; }

        /// <summary>Maximum gauge value.</summary>
        public float MaxValue => Core.GameConstants.DIVINITY_GAUGE_MAX;

        /// <summary>Whether the gauge is full and G-Mode can be activated.</summary>
        public bool IsReady => CurrentValue >= MaxValue;

        /// <summary>Gauge fill percentage (0.0 to 1.0).</summary>
        public float FillPercent => Mathf.Clamp01(CurrentValue / MaxValue);

        /// <summary>Event fired when gauge reaches 100.</summary>
        public event System.Action OnGaugeReady;

        /// <summary>Event fired when gauge value changes.</summary>
        public event System.Action<float> OnGaugeChanged;

        public DivinityGauge()
        {
            CurrentValue = 0f;
        }

        /// <summary>
        /// Add energy from a player chain result.
        /// </summary>
        public void AddFromChain(ChainResult chain)
        {
            Add(chain.DivinityGenerated);
        }

        /// <summary>
        /// Add energy from a cascade match.
        /// </summary>
        public void AddFromCascade()
        {
            Add(Core.GameConstants.DIVINITY_PER_SKYFALL_COMBO);
        }

        /// <summary>
        /// Add energy from hitting with elemental advantage.
        /// </summary>
        public void AddFromAdvantageHit()
        {
            Add(Core.GameConstants.DIVINITY_PER_ADVANTAGE_HIT);
        }

        /// <summary>
        /// Add energy from taking damage.
        /// Amount = (damagePercent × 10), where damagePercent = damage / maxHP.
        /// </summary>
        public void AddFromDamageTaken(float damagePercent)
        {
            Add(damagePercent * 10f);
        }

        /// <summary>
        /// Consume the gauge when Divinity Burst is activated.
        /// Returns false if gauge isn't full.
        /// </summary>
        public bool Consume(float cost)
        {
            if (CurrentValue < cost)
                return false;

            CurrentValue -= cost;
            OnGaugeChanged?.Invoke(CurrentValue);
            return true;
        }

        /// <summary>
        /// Consume full gauge for a Divinity Burst (standard cost = 100).
        /// </summary>
        public bool ConsumeForBurst()
        {
            return Consume(Core.GameConstants.DIVINITY_GAUGE_MAX);
        }

        /// <summary>
        /// Reset gauge for new battle.
        /// </summary>
        public void Reset()
        {
            CurrentValue = 0f;
            OnGaugeChanged?.Invoke(CurrentValue);
        }

        private void Add(float amount)
        {
            float oldValue = CurrentValue;
            CurrentValue = Mathf.Min(CurrentValue + amount, MaxValue);
            OnGaugeChanged?.Invoke(CurrentValue);

            // Fire ready event if we just hit 100
            if (!IsReadyAt(oldValue) && IsReady)
            {
                OnGaugeReady?.Invoke();
            }

            Debug.Log($"[Divinity] +{amount:F1} → {CurrentValue:F1}/{MaxValue}");
        }

        private bool IsReadyAt(float value) => value >= MaxValue;
    }
}
