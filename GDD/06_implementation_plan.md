# 《神魔：極限演武》Agent Execution Plan v2.0

> **v2.0 Redesign** — Core mechanics changed from phantom trail to **Orb Echo (元素律動)** + **Divinity Burst (神格覺醒)**
> Based on full GDD analysis + user mechanic specifications

---

## Key Mechanic Changes from v1.0

| Aspect | v1.0 (Old) | v2.0 (New) |
|--------|-----------|-----------|
| Orb System | 5s drag timer + phantom trail | Strategic Orb Echo connections (3/5/7+ chains) |
| Action Economy | Free-form drag | 3 Action Points per turn |
| Skills | Generic 4-layer (Leader/Active/Passive/Exceed) | Per-character Orb Echo + Passive + Divinity Burst |
| Ultimate | Awakening gauge → generic burst | Divinity Gauge → cinematic character-specific ultimate |
| Progression | Fragment synthesis + gacha-like | Achievement-based unlock + Soul Armament |
| Numbers | Uncapped inflation | Strict ATK 5000-8000, Boss HP 1M-10M |

---

## Agent Roles (unchanged)

| Agent | Scope |
|-------|-------|
| 🔧 Engine | Unity, URP, rendering, performance |
| 🎮 Gameplay | Orb Echo, combat, state machines, AI |
| 🎨 Art | Shaders, VFX, particles, animations |
| 📐 Level | Maps, environments, terrain mechanics |
| 🖥️ UI | Menus, HUD, accessibility |
| 📖 Narrative | Dialogue, quests, cutscenes |
| 🔗 Backend | Save system, cloud sync, CI/CD |
| 🧪 QA | Testing, profiling, balance |

---

## Phase 1: MVP Prototype (Month 0-6)

### WP-1.1: Unity Project Scaffolding ✅ COMPLETE

---

### WP-1.2: Orb Echo Board System (REDESIGNED)
**Agent**: 🎮 Gameplay + 🎨 Art
**GDD**: [01_combat_system.md](file:///d:/antigravitty/TOS/GDD/01_combat_system.md) §2

**Tasks (Gameplay — 3 sessions)**:
1. **Board Data**: 5×6 grid, `OrbType` enum, random fill, skyfall gravity
2. **Connection System**: Tap-and-drag to connect adjacent same-color orbs; path validation (no diagonal, no overlap)
3. **Chain Length Detection**: Count connected orbs → classify as 3-chain / 5-chain / 7+-chain
4. **Action Points**: 3 AP/turn, consume 1/2/3 AP per chain length
5. **Combo System**: Track chains per turn, apply +10/20/30% team damage bonus
6. **Skyfall/Cascade**: After elimination, fill from above (or below if gravity inverted); auto-match cascades = bonus combos
7. **Board Events**: Locked orbs (unpinnable, still matchable), poison orbs, boss-injected orbs
8. **Divinity Gauge**: Shared 0–100 meter, charges on chains/damage taken

**Tasks (Art — 1 session)**:
1. Chain connection line VFX (glowing element-colored path)
2. Orb elimination burst particles
3. Skyfall animation + cascade chain lightning effects
4. Divinity Gauge fill animation (dramatic buildup)

**Deliverables**: Full orb board with chain connections, AP, combos, skyfall, gauge
**Dependencies**: WP-1.1
**Effort**: 4 sessions

---

### WP-1.3: Combat Engine with Orb Echo (REDESIGNED)
**Agent**: 🎮 Gameplay
**GDD**: [01_combat_system.md](file:///d:/antigravitty/TOS/GDD/01_combat_system.md) §3,5

**Tasks (2 sessions)**:
1. **Turn Flow**: `PlayerPhase (3 AP) → ResolutionPhase (animate skills) → EnemyPhase`
2. **Orb-to-Character Mapping**: Chain element → matching character triggers Orb Echo skill
3. **Damage Formula**: `ATK × SkillMultiplier × AttributeAdvantage × ComboBonus × PassiveBonus × EnvModifier - DEF`
4. **Team System**: 5-member party, element priority ordering
5. **Heart Chain**: Maps to team-wide heal based on RCV total
6. **Enemy AI (Basic)**: Attack telegraph, single-pattern boss
7. **Status Effects**: Freeze, slow, poison, shield, stun, undying
8. **Character Priority**: When multiple chars share element, leader acts first

**Deliverables**: Full battle loop with Orb Echo → character skills → enemy turn
**Dependencies**: WP-1.2
**Effort**: 2 sessions

---

### WP-1.4: Cel-Shading Renderer (unchanged)
**Effort**: 2 sessions

---

### WP-1.5: Character Orb Echo Skills (REDESIGNED)
**Agent**: 🎮 Gameplay
**GDD**: [01_combat_system.md](file:///d:/antigravitty/TOS/GDD/01_combat_system.md) §3

**Tasks (2 sessions)**:
1. **OrbEchoSkillSO**: ScriptableObject — 3-chain effect, 5-chain effect, 7+-chain combined effect
2. **PassiveTraitSO**: Condition-based persistent effect per character
3. **DivinityBurstSO**: G-mode ultimate, animation trigger, full-screen effect
4. **MVP Characters (3)**:
   - **Molly**: 3-chain=Water Barrage (3× small hits + Slow), 5-chain=Azure Lance (row heavy + crit if slowed), Burst=Holy Lance: Sea God Judgment (全體 + 100% freeze)
   - **Satan**: 3-chain=Demon Slash (AOE dark), 5-chain=Soul Sacrifice (-20% HP, scaling dark AOE), Burst=Terminus: Endless Void (sacrifice 50% team HP → 10× true damage)
   - **Thor**: 3-chain=Thunder Strike (single light hit), 5-chain=Mjolnir Slam (column + stun), Burst=Ragnarok Thunder (全體 light + shield)
5. **Skill Resolution Pipeline**: Queue Orb Echo results → animate in sequence → apply effects

**Deliverables**: 3 characters with unique Orb Echo + Passive + Divinity Burst
**Dependencies**: WP-1.3
**Effort**: 2 sessions

---

### WP-1.6: Test Level — Mechanical City Core (REDESIGNED)
**Agent**: 📐 Level
**GDD**: [01_combat_system.md](file:///d:/antigravitty/TOS/GDD/01_combat_system.md) §Appendix

**Tasks (2 sessions)**:
1. **Scene**: Gear/steam/circuit aesthetic ancient ruin interior
2. **Gravity Inversion**: Every 3 turns, skyfall direction flips (bottom-up)
3. **Circuit Overload**: 3× Light 5-chains = light circuit pillars → Boss enters "short-circuit" (DEF=0, 1 turn)
4. **Boss AI — Gyroscope**: Phase 1 (light shield), Phase 2 (pin 4 orbs), Phase 3 (5-turn self-destruct → require G Mode kill)
5. **NavMesh / positioning**: Character positions on 3D field affect AOE targeting

**Deliverables**: Playable test level with environment↔board interaction
**Dependencies**: WP-1.4
**Effort**: 2 sessions

---

### WP-1.7: MVP UI Framework (minor updates)
**Effort**: 2 sessions (added: AP counter, Divinity Gauge bar, chain preview)

---

### WP-1.8: Save System (unchanged)
**Effort**: 1 session

---

### **Phase 1 Total**: ~17 sessions

---

## Phase 2: Alpha (Month 6-12)

### WP-2.1: Full Character Roster (20 chars)
**Tasks**: Implement 17 more characters with unique Orb Echo + Passive + Divinity Burst
**Effort**: 3 sessions

### WP-2.2: Advanced Combat & Boss Mechanics
**Tasks**: Multi-phase bosses, board manipulation (locked/poison/bomb orbs), combo shields, attribute shields, enrage timers
**Effort**: 2 sessions

### WP-2.3: Environment Systems
**Tasks**: Gravity variants, elemental terrain → board orb-rate changes, weather effects, day/night
**Effort**: 4 sessions

### WP-2.4: 3 Battle Maps + 4 Difficulties each
**Effort**: 3 sessions

### WP-2.5: Boss Fight — Gyroscope (Full) + Tianli/Destiny
**Effort**: 2 sessions

### WP-2.6: Narrative System & Story (Ch.0-3)
**Effort**: 3 sessions

### WP-2.7: Progression System (REDESIGNED)
**Tasks**: Achievement-based character unlock, deterministic upgrade materials, Soul Armament crafting, daily dungeons (100% drop)
**Effort**: 2 sessions

### WP-2.8: Cross-Platform Input & Quality
**Effort**: 1 session

### WP-2.9: A/B Testing (updated for Orb Echo)
**Tasks**: Test 3 AP vs 4 AP, chain time limits, combo bonus curve
**Effort**: 1 session

### **Phase 2 Total**: ~21 sessions

---

## Phase 3: Beta (Month 12-18)

### WP-3.1-3.9: (structure same as v1.0, updated for Orb Echo skills per character)
### **Phase 3 Total**: ~19 sessions

---

## Phase 4: Release (Month 18-24)

### WP-4.1-4.4: (unchanged)
### **Phase 4 Total**: ~6 sessions

---

## Grand Total

| Phase | Sessions | Milestone |
|-------|----------|-----------|
| MVP | ~17 | Demo with Orb Echo + 3 characters |
| Alpha | ~21 | 20 chars, 15 stages, closed beta |
| Beta | ~19 | 50 chars, 30 stages, public beta |
| Release | ~6 | v1.0 launch |
| **Total** | **~63** | — |

---

## Verification Plan (updated)

### Unit Tests
- Chain connection path validation (no diagonal, no overlap, same color)
- AP consumption per chain length
- Damage formula: ATK × multiplier × advantage × combo
- Divinity Gauge accumulation
- Skyfall gravity inversion

### Playmode Tests
- Full turn: 3 AP → chain selections → resolution → enemy → repeat
- Orb Echo skill triggering per character
- Boss phase transitions
- Environment ↔ board interaction (gravity flip, circuit overload)

### Manual
- Chain selection feel: responsive, satisfying connection line feedback
- Boss fight pacing: Phase 1→2→3 transition feels dramatic
- FPS: ≥30fps on Snapdragon 778G during Divinity Burst animation
