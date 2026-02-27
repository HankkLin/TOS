// ============================================================
// GameEnums.cs — Core enumerations for Tower of Saviors: TOS
// GDD Source: 01_combat_system.md §1, 02_narrative_worldbuilding.md §1
// ============================================================

namespace TOS.Core
{
    /// <summary>
    /// The five elemental attributes plus Heart (recovery).
    /// Water > Fire > Wood > Water (cycle), Light ⇄ Dark (mutual).
    /// </summary>
    public enum Element
    {
        Water,
        Fire,
        Wood,
        Light,
        Dark,
        Heart
    }

    /// <summary>
    /// Orb types on the 5×6 orb board.
    /// Extends Element with special orb types from boss/environment mechanics.
    /// </summary>
    public enum OrbType
    {
        Water,
        Fire,
        Wood,
        Light,
        Dark,
        Heart,
        // Special orbs (GDD: 03_level_environment.md §2)
        Locked,         // Cannot be moved
        Poison,         // Elimination reduces ATK
        Unknown,        // ? orb — unknown element until revealed
        Bomb,           // Damages player on elimination
        Enhanced        // Stronger version of base element orb
    }

    /// <summary>
    /// Character rarity tiers.
    /// </summary>
    public enum Rarity
    {
        N,    // 1-2 star
        R,    // 3 star
        SR,   // 4 star
        SSR   // 5-6 star
    }

    /// <summary>
    /// Character race/species types.
    /// GDD: 02_narrative_worldbuilding.md §1.2
    /// </summary>
    public enum Race
    {
        Human,
        God,
        Demon,
        Dragon,
        Beast,
        Fairy,
        Machina
    }

    /// <summary>
    /// Skill layer types — progressively unlocked per GDD §4.1.
    /// </summary>
    public enum SkillType
    {
        LeaderSkill,    // Passive aura — unlocked at start
        ActiveSkill,    // CD-based — unlocked at start
        PassiveSkill,   // Trigger-based — unlocked at player Lv31
        ExceedSkill     // Ultimate — unlocked at player Lv61
    }

    /// <summary>
    /// Orb board state machine states.
    /// GDD: 04_technical_architecture.md §C.3
    /// </summary>
    public enum BoardState
    {
        Idle,           // Waiting for player input
        PlayerInput,    // Player is dragging an orb
        Calculating,    // Finding matches after drag ends
        Eliminating,    // Playing elimination animations
        Refilling,      // Dropping remaining orbs + spawning new ones
        ChainCheck      // Checking for cascade combos after refill
    }

    /// <summary>
    /// Battle phase in the hybrid turn system.
    /// GDD: 01_combat_system.md §3.3
    /// </summary>
    public enum BattlePhase
    {
        PlayerTurn,     // Strategic planning + orb manipulation (time frozen)
        Execution,      // Attacks resolve in real-time (3s)
        EnemyPhase      // Enemy actions + 2s player reaction window
    }

    /// <summary>
    /// Difficulty levels for stages.
    /// GDD: 03_level_environment.md §Difficulty
    /// </summary>
    public enum Difficulty
    {
        Normal,
        Hard,
        Hell,
        Nightmare
    }

    /// <summary>
    /// Match pattern types detected during orb elimination.
    /// GDD: 01_combat_system.md §2.2
    /// </summary>
    public enum MatchPattern
    {
        ThreeMatch,     // 3 orbs — single target attack
        FourMatch,      // 4 orbs — small AOE (2×2)
        FiveMatch,      // 5+ orbs — full party attack + enhanced orb
        CrossMatch,     // Cross shape — cross AOE + penetrate
        LTMatch         // L or T shape — fan attack
    }

    /// <summary>
    /// Gravity states for environment system.
    /// GDD: 03_level_environment.md §1
    /// </summary>
    public enum GravityState
    {
        Standard,   // 1G
        Low,        // 0.3G
        High,       // 2G
        Reverse     // -1G
    }

    /// <summary>
    /// Weather types for the dynamic weather system.
    /// GDD: 03_level_environment.md §4.1
    /// </summary>
    public enum WeatherType
    {
        Clear,
        Rain,
        Storm,
        Snow,
        Sandstorm,
        BloodMoon
    }

    /// <summary>
    /// Time of day for day/night cycle.
    /// GDD: 03_level_environment.md §4.2
    /// </summary>
    public enum TimeOfDay
    {
        Day,        // 06:00-18:00
        Dusk,       // 18:00-20:00
        Night       // 20:00-06:00
    }

    /// <summary>
    /// Device performance tier for quality settings.
    /// GDD: 04_technical_architecture.md §E.2
    /// </summary>
    public enum DeviceTier
    {
        Legacy,     // Snapdragon 6xx, A12 — 30fps@720p minimal mode
        Low,        // Snapdragon 778G, A13 — 30fps@1080p
        Medium,     // Snapdragon 778G+, A14 — 60fps@1080p
        High,       // Snapdragon 8 Gen 2+, A16+ — 60fps+
        Ultra       // PC / flagship — 144fps@1440p
    }

    /// <summary>
    /// Character variant categories.
    /// GDD: 02_narrative_worldbuilding.md §5.2
    /// </summary>
    public enum VariantCategory
    {
        Original,           // Base version
        HistoricalDivergence,  // Different choice at key moment
        EraEvolution,       // Different time period version
        Fusion,             // Two+ characters merged
        FutureEcho,         // From a possible future
        Special             // Story-specific unique
    }
}
