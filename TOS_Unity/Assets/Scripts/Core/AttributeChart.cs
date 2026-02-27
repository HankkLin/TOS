// ============================================================
// AttributeChart.cs — Element advantage/disadvantage lookup
// GDD Source: 01_combat_system.md §1.1
// ============================================================

namespace TOS.Core
{
    /// <summary>
    /// Provides element-vs-element damage multiplier lookups.
    /// Water > Fire > Wood > Water (cycle)
    /// Light ⇄ Dark (mutual advantage)
    /// Heart = recovery only, no combat advantage
    /// </summary>
    public static class AttributeChart
    {
        /// <summary>
        /// Returns the damage multiplier when attacker's element hits defender's element.
        /// </summary>
        public static float GetMultiplier(Element attacker, Element defender)
        {
            if (attacker == Element.Heart || defender == Element.Heart)
                return GameConstants.ATTRIBUTE_NEUTRAL;

            // Cycle: Water > Fire > Wood > Water
            if ((attacker == Element.Water && defender == Element.Fire) ||
                (attacker == Element.Fire  && defender == Element.Wood) ||
                (attacker == Element.Wood  && defender == Element.Water))
            {
                return GameConstants.ATTRIBUTE_ADVANTAGE;
            }

            // Reverse cycle
            if ((attacker == Element.Fire  && defender == Element.Water) ||
                (attacker == Element.Wood  && defender == Element.Fire)  ||
                (attacker == Element.Water && defender == Element.Wood))
            {
                return GameConstants.ATTRIBUTE_DISADVANTAGE;
            }

            // Mutual: Light ⇄ Dark
            if ((attacker == Element.Light && defender == Element.Dark) ||
                (attacker == Element.Dark  && defender == Element.Light))
            {
                return GameConstants.ATTRIBUTE_ADVANTAGE;
            }

            return GameConstants.ATTRIBUTE_NEUTRAL;
        }

        /// <summary>
        /// Returns true if the attacker has elemental advantage over defender.
        /// </summary>
        public static bool HasAdvantage(Element attacker, Element defender)
        {
            return GetMultiplier(attacker, defender) > GameConstants.ATTRIBUTE_NEUTRAL;
        }

        /// <summary>
        /// Maps an OrbType to its corresponding Element (for combat damage calculation).
        /// </summary>
        public static Element OrbToElement(OrbType orbType)
        {
            return orbType switch
            {
                OrbType.Water    => Element.Water,
                OrbType.Fire     => Element.Fire,
                OrbType.Wood     => Element.Wood,
                OrbType.Light    => Element.Light,
                OrbType.Dark     => Element.Dark,
                OrbType.Heart    => Element.Heart,
                OrbType.Enhanced => Element.Heart, // Enhanced inherits from context
                _ => Element.Heart // Special orbs don't have combat element
            };
        }
    }
}
