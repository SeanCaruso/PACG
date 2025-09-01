using System;
using System.Collections.Generic;
using System.Linq;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ContextManager
    {
        private ActionStagingManager _asm;
        private CardManager _cardManager;
        private GameServices _gameServices;

        public void Initialize(GameServices gameServices)
        {
            _asm = gameServices.ASM;
            _cardManager = gameServices.Cards;
            _gameServices = gameServices;
        }

        // ======================================================================
        // THE CONTEXTS
        // ======================================================================
        public GameContext GameContext { get; private set; }
        public TurnContext TurnContext { get; private set; }
        public EncounterContext EncounterContext { get; private set; }
        public CheckContext CheckContext { get; private set; }
        public IResolvable CurrentResolvable { get; private set; }

        // ======================================================================
        // STARTING / ENDING CONTEXTS
        // ======================================================================

        public void NewGame(GameContext gameContext) => GameContext = gameContext;

        public void NewTurn(TurnContext turnContext)
        {
            TurnContext = turnContext;
            GameContext.ActiveCharacter = turnContext.Character;
        }

        public void EndTurn()
        {
            if (CheckContext != null)
                Debug.LogWarning($"[{GetType().Name}] Ending turn with a CheckContext still active!");
            if (EncounterContext != null)
                Debug.LogWarning($"[{GetType().Name}] Ending turn with an EncounterContext still active!");

            TurnContext = null;
        }

        // TODO: Think about how this will work in nested encounters - maybe use a stack?
        public void NewEncounter(EncounterContext encounterContext)
        {
            EncounterContext = encounterContext;

            encounterContext.ExploreEffects.AddRange(TurnContext?.ExploreEffects ?? new List<IExploreEffect>());
            TurnContext?.ExploreEffects.Clear();
        }

        /// <summary>
        /// This only sets the context to null. Event sending must be handled by the caller.
        /// </summary>
        public void EndEncounter() => EncounterContext = null;

        /// <summary>
        /// USE ONLY IF YOU KNOW WHAT YOU'RE DOING! NewResolvableProcessor IS PROBABLY BETTER!
        /// </summary>
        /// <param name="resolvable"></param>
        public void NewResolvable(IResolvable resolvable)
        {
            if (CurrentResolvable != null)
                Debug.LogWarning($"[ContextManager] Created {resolvable} is overwriting {CurrentResolvable}!");

            foreach (var action in EncounterContext?.ResolvableModifiers ?? new List<Action<IResolvable>>())
            {
                action(resolvable);
            }

            // If this is a damage resolvable, check to see if we have any responses for it. If so, we'll need to
            // handle those responses first.
            if (resolvable is DamageResolvable damageResolvable)
            {
                var args = new DiscardEventArgs(
                    damageResolvable.PlayerCharacter,
                    new List<CardInstance>(),
                    CardLocation.Hand,
                    damageResolvable
                );
                _cardManager.TriggerBeforeDiscard(args);

                if (args.HasResponses)
                {
                    var options = args.CardResponses.Select(response =>
                        new PlayerChoiceResolvable.ChoiceOption(response.Description, response.OnAccept)
                    ).ToList();
                    options.Add(new PlayerChoiceResolvable.ChoiceOption("Skip", () => { }));

                    var choices = new PlayerChoiceResolvable("Use Power?", options.ToArray());
                    var damageProcessor = new NewResolvableProcessor(damageResolvable, _gameServices);
                    choices.OverrideNextProcessor(damageProcessor);

                    var choiceProcessor = new NewResolvableProcessor(choices, _gameServices);
                    _gameServices.GameFlow.StartPhase(choiceProcessor, "Power Options");
                    return;
                }
            }

            CurrentResolvable = resolvable;

            // Automatic context creation based on resolvable type.
            if (CurrentResolvable is CheckResolvable checkResolvable)
            {
                CheckContext = new CheckContext(checkResolvable);
                DialogEvents.RaiseCheckStartEvent(CheckContext);

                CheckContext.ExploreEffects.AddRange(EncounterContext?.ExploreEffects ?? new List<IExploreEffect>());

                EncounterContext?.ExploreEffects.RemoveAll(effect =>
                    effect is SkillBonusExploreEffect { IsForOneCheck: true });
            }

            // Now that it's set as our current resolvable and we have a CheckContext if needed,
            // do any post-construction setup.
            resolvable.Initialize();

            // Update the UI.
            GameEvents.RaiseTurnStateChanged();
            _asm.UpdateGameStatePreview();
            _asm.UpdateActionButtons();
        }

        /// <summary>
        /// THIS SHOULD ONLY BE CALLED BY ActionStagingManager!
        /// </summary>
        public void EndResolvable()
        {
            CurrentResolvable?.Resolve();
            CurrentResolvable = null;
            GameEvents.RaiseTurnStateChanged();
        }

        public void EndCheck()
        {
            DialogEvents.RaiseCheckEndEvent();
            CheckContext = null;
            GameEvents.RaiseTurnStateChanged();
        }

        // ======================================================================
        // CONVENIENCE FUNCTIONS
        // ======================================================================

        // Get relevant locations
        public Location TurnPcLocation => GameContext?.GetPcLocation(TurnContext?.Character);
        public Location EncounterPcLocation => GameContext?.GetPcLocation(EncounterContext?.Character);

        // Test for additional explorations
        public bool AreCardsPlayable => // Generic cards are playable if...
        CurrentResolvable == null // ... we don't have a resolvable...
         && EncounterContext == null // ... or encounter...
         && _asm.StagedActions.Count == 0; // ... and we don't have any currently staged actions.
        
        public bool IsExplorePossible => // Exploration is possible if...
            AreCardsPlayable // ... we can play cards in general...
            && TurnPcLocation.Count > 0; // ... and we have more cards in the location.
    }
}
