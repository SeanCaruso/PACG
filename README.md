# PACG (Pathfinder Adventure Card Game) - Digital Implementation

A Unity-based digital implementation of the Pathfinder Adventure Card Game.

---

## TABLETOP GAME RULES OVERVIEW
This section describes the rules for the tabletop version of the game.

### Scenario Setup (FUTURE IMPLEMENTATION)
- Select a scenario and read its objectives and special rules.
- Build the location decks and hourglass timer.
- Distribute characters, starting decks, and hands.
- Place characters at starting locations.

### Turn Structure (CURRENTLY BEING WORKED ON)
Each player's turn follows this sequence:

1. **Advance Time**: Advance the Hour (a special deck of Blessing cards).
2. **Give a Card (Optional)**: Give a card to another local character.
3. **Move (Optional)**: Move to a distant location.
4. **Explore (Optional)**: Reveal the top card of the location and encounter it (see below).
5. **Close Location**: Attempt to close it if it's empty.
6. **End Turn**: Reset hand, recover cards, pass turn.

### Encountering a Card (PARTIALLY FUNCTIONAL)
- Apply any immediate encounter effects.
- Attempt the relevant check (acquire or defeat).
- On success, gain the card or remove the threat.
- On failure, apply consequences (e.g., damage).

### Making a Check (SHOULD BE MOSTLY FUNCTIONAL)
- Determine the skill used and the check difficulty.
- Play cards and apply modifiers.
- Roll dice and compare the result to the target number.
- Apply success or failure outcomes.

### Playing Cards
A card can be played whenever the card allows it. Some cards specify when they can be used—such as "before acting," "on a check," or "after acting."

- **During Your Turn**: You may play cards freely between phases (except where timing restrictions apply).
- **During an Encounter**: Cards must relate directly to the current step (e.g., only play cards that affect checks during the check step).
- **One Per Type Rule**: Only one of each type of boon (e.g., weapon, spell, blessing) can be played on a single check, unless a card says it can be played freely.
- **Card Actions**: Playing a card may involve revealing, discarding, recharging, burying, or banishing it, depending on its effect.

Always follow the card’s instructions precisely—if you can't do everything the power you're using requires, you can't play it.

---

## Tabletop to Digital Considerations
Implementing the rules verbatim would kinda suck. These are some considerations I made to make things more enjoyable.

### Action Staging
- Instantly committing any player decisions means no take-backsies. That's no fun.
- In most cases, when someone wants to do something with a card, that action is staged.
- The user can cancel - this unstages all staged actions.
- If staged actions satisfy any current resolvables, the user can commit the actions.

### Turn Structure
- All valid turn phases are available at the start of a turn.
- They must be done in turn phase order. E.g., if the player explores, they can no longer give a card or move.

---

## 🏗️ ARCHITECTURE DECISIONS

### Current State
- **Status**: Processor architecture with resolvable pause/resume system implemented
- **Removed**: Coroutine-based managers (TurnManager, EncounterManager), ServiceLocator pattern
- **Implemented**: GameFlowManager processor queue with resolvable handling, GameServices dependency bundling, resolvable-based card logic
- **Focus**: Testing end-to-end game flow and completing remaining processors

### Layer Architecture
```
PACG.Gameplay    → Pure C# game rules, managers, card logic, contexts
PACG.Presentation → Unity UI components, display logic
PACG.Data        → ScriptableObject definitions
PACG.SharedAPI   → Cross-layer view controllers, events, view models
```

### Key Patterns
- **GameFlowManager Pause/Resume**: Central processor coordination with automatic pause on resolvables requiring user input
- **Processor Pattern**: Game actions implemented as discrete `IProcessor` instances
- **GameServices Two-Phase Init**: Dependency injection bundle with delayed LogicRegistry registration to avoid circular dependencies
- **Resolvable System**: Card logic declares requirements (CombatResolvable, DamageResolvable) that pause GFM for user input
- **Automatic Context Creation**: ContextManager creates CheckContext automatically for ICheckResolvable types
- **Action Staging with Request-Validate-Commit-Notify Pattern**: ActionStagingManager serves as single source of truth for staged actions, with CheckContext handling validation and check-specific rules
- **CardManager Single Source of Truth**: All card location queries and updates go through CardManager
- **Event-Driven UI Updates**: CardLocationChanged events drive UI display updates (Gameplay → Presentation only)
- **Lazy Card Display Instantiation**: CardDisplayController creates CardDisplay objects on-demand
- **Events**: `GameEvents` static class for decoupled communication (in `PACG.SharedAPI/`)
- **ViewModels**: `CardViewModel` separates presentation data from domain objects (in `PACG.SharedAPI/`)
- **Unified Card Logic System**: Single `CardLogicBase` inheritance (no interfaces), unified `LogicRegistry.GetCardLogic()` for all card types, attribute-based registration

### Planned: Hybrid Card Logic Pattern
For complex conditional effects (e.g., "If undefeated, do X"):
- **Primary Pattern**: Cards queue all resolvables upfront via `CardLogic.Execute()`
- **Conditional Extension**: Cards implement `IConditionalCardLogic.OnResolvableCompleted()` 
- **Reactive Queuing**: Additional resolvables queued based on resolution outcomes
- **Benefit**: Maintains declarative simplicity while handling conditional card powers

---

## 🚧 KNOWN ISSUES & PLANNED CHANGES

### Current Priorities
1. **Test resolvable flow** - Verify complete encounter cycle from card logic to user input to resolution
2. **Complete processor implementations** - Many turn action processors are currently placeholders
3. **Complete remaining processors** - Implement processors for non-combat resolvables (damage, acquire, etc.)
4. **Encounter initialization** - ExploreProcessor doesn't properly set up encounter context
5. **Turn end logic** - EndTurn functionality not connected to processor pattern
6. **Resolvable coverage** - Only 3 resolvable types have processor mappings
7. **End-to-end integration testing** - Full turn and encounter cycles with actual user interaction
8. **Conditional card effects** - Implement hybrid pattern for "If undefeated" type effects
9. **Polish UI feedback** - Ensure proper button states and user prompts during resolvable resolution

### Technical Constraints
- **Unity Version**: 6000.1.11f1
- **Build System**: Unity-only (no external build scripts)
- **Input**: Unity Input System package
- **Data Persistence**: ScriptableObjects

---

## 🎯 DEVELOPMENT APPROACH

### Current Focus
**"Get it working, then make it clean"** - Implementing core game loop before architectural perfection.

### Architectural Philosophy
- **Pragmatic over purist** - Clean architecture serves development speed, not vice versa
- **Single Responsibility** - But cohesive controllers are better than micro-classes
- **Layer separation** - Core never depends on Presentation/Unity
- **No premature abstraction** - Build what's needed now, not what might be needed
- **Keep the future in mind** - But don't build ourselves into a corner for later features

### Testing Strategy
- **Manual testing** through Unity play mode
- **No automated tests** currently (solo development prioritizing features)

---

## 🤔 DECISION BACKLOG

### Game Design
- [ ] Turn phase enforcement strategy
- [ ] Multiple actions per turn rules  
- [ ] Turn end conditions
- [ ] Damage resolution flow
- [ ] Card acquisition mechanics

### Technical Architecture - Active Decisions
- [ ] Conditional card effect implementation strategy (hybrid pattern)
- [ ] Testing strategy for complex game interactions
- [ ] Performance considerations for card display

---

*This README should be updated whenever design decisions are made or architecture changes.*