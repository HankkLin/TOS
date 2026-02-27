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
    /// Orb board state machine states — Orb Echo flow.
    /// GDD: 01_combat_system.md v2.0 §2
    /// </summary>
    public enum BoardState
    {
        Idle,           // Waiting for player to start a chain
        ChainInput,     // Player is connecting same-color orbs
        Resolving,      // Evaluating chain length → trigger Orb Echo skill
        Eliminating,    // Playing elimination + skill animations
        Skyfall,        // Dropping/filling new orbs (gravity-aware)
        ChainCheck      // Checking for cascade combos after skyfall
    }

    /// <summary>
    /// Battle phase in the turn system.
    /// GDD: 01_combat_system.md v2.0 §5.1
    /// </summary>
    public enum BattlePhase
    {
        PlayerPhase,    // Player uses 3 AP to make chain connections
        Resolution,     // Orb Echo skills animate and resolve in sequence
        EnemyPhase      // Enemy attacks + environment mechanics trigger
    }

    /// <summary>
    /// Chain length classification for Orb Echo.
    /// GDD: 01_combat_system.md v2.0 §2.2
    /// </summary>
    public enum ChainLength
    {
        None,           // < 3 connections (invalid)
        Basic,          // 3 connections → basic Orb Echo skill (1 AP)
        Enhanced,       // 5 connections → enhanced Orb Echo skill (2 AP)
        Ultimate        // 7+ connections → combined basic+enhanced+extra (3 AP)
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
    /// Orb Echo skill trigger type — per-character unique.
    /// GDD: 01_combat_system.md v2.0 §3.1
    /// </summary>
    public enum OrbEchoTier
    {
        BasicEcho,      // 3-chain triggers character's basic echo skill
        EnhancedEcho,   // 5-chain triggers character's enhanced echo skill
        UltimateEcho    // 7+-chain triggers basic + enhanced + extra
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
