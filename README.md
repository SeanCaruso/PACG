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
- **Turn Phase Ordering**: Phases must be completed in sequence
- **Pause/Resume System**: Game pauses for user input, resumes after decisions

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
- **GameServices**: Dependency injection bundle for simplified parameter passing
- **Action Staging**: Request-Validate-Commit-Notify pattern with undo support
- **Card Logic Integration**: Logic stored on `CardInstance` with facade pattern
- **Event-Driven UI**: CardLocationChanged events (Gameplay → Presentation)
- **Convention-Based Discovery**: `{cardID}Logic` class names for card behavior

## Current Status

### Priorities
1. **Complete processor implementations** - Many turn action processors are placeholders
2. **Fix encounter initialization** - ExploreProcessor context setup issues
3. **End-to-end testing** - Verify complete turn and encounter cycles

### Technical Info
- **Unity Version**: 6000.1.11f1
- **Testing**: Manual play mode testing
- **Data**: ScriptableObjects

## Development Approach

- **Philosophy**: Get it working, then make it clean
- **Architecture**: Layer separation (Core never depends on Presentation)
- **Testing**: Manual testing through Unity play mode
- **Focus**: Core game loop implementation