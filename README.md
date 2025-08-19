# PACG (Pathfinder Adventure Card Game) - Digital Implementation

A Unity-based digital implementation of the Pathfinder Adventure Card Game.

## Game Overview

### Turn Structure
1. **Advance Time** → **Give Card** → **Move** → **Explore** → **Close Location** → **End Turn**
2. **Encountering Cards**: Apply effects, attempt checks, resolve success/failure
3. **Making Checks**: Determine skill/difficulty, play cards, roll dice, apply outcomes
4. **Card Actions**: Reveal, discard, recharge, bury, or banish based on card effects

## Digital Implementation Features

- **Action Staging**: Player actions are staged before commitment, allowing undo/cancel
- **Resolvable System**: Complex interactions broken into user-resolvable steps
- **Dynamic UI**: Character powers and choices adapt based on game state
- **Event-Driven Updates**: Clean separation between game logic and presentation

## Architecture Overview

### Layer Structure
```
PACG.Gameplay    → Pure C# game rules, managers, card logic, contexts
PACG.Presentation → Unity UI components, display logic
PACG.Data        → ScriptableObject definitions
PACG.SharedAPI   → Cross-layer view controllers, events, view models
```

### Key Patterns
- **Hierarchical Processor Pattern**: Game actions as `IProcessor` instances with `Stack<Queue<IProcessor>>` for proper phase nesting
- **Resolvable Architecture**: `BaseResolvable` system for user interactions requiring input
- **GameServices**: Dependency injection bundle for simplified parameter passing
- **Action Staging**: Request-Validate-Commit-Notify pattern with undo support
- **Card Logic Integration**: Logic stored on `CardInstance` with facade pattern
- **Convention-Based Discovery**: `{cardID}Logic` class names for card behavior
- **Player Choice System**: Dynamic choice presentation with structured options

## Current Status

### Current Priorities
1. **Expand card implementations** - Add more card-specific logic
2. **Enhanced encounter variety** - Implement location-specific behaviors
3. **End-to-end testing** - Verify complete game scenarios
4. **Advanced check types** - Beyond basic combat checks

### Cards to Test
**Spells**
- Enchant Weapon - Spell effects and recovery.
 
**Weapons**
- Quarterstaff - Combat checks and Obstacle/Trap barrier evasion.

### Technical Info
- **Unity Version**: 6000.1.11f1
- **Testing**: Manual play mode testing
- **Data**: ScriptableObjects

## Development Approach

- **Architecture**: Clean layer separation with dependency injection
- **Patterns**: Event-driven UI updates, resolvable-centric interactions
- **Testing**: Manual testing through Unity play mode
- **Focus**: Robust core systems supporting complex card interactions