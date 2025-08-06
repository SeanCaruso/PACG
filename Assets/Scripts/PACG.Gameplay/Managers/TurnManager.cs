
using PACG.Presentation;
using PACG.SharedAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Gameplay
{
    public class TurnManager
    {
        private Deck locationDeck = new();

        // ==== TURN PHASE AVAILABILITY ============================
        private bool canGive = false;
        public bool CanGive => canGive;

        private bool canMove = false;
        public bool CanMove => canMove;

        private bool canExplore = false;
        public bool CanExplore => canExplore;

        private bool canCloseLocation = false;
        public bool CanCloseLocation => canCloseLocation;

        private bool canEndTurn = false;
        public bool CanEndTurn => canEndTurn;

        // ==== RETRIEVABLE PROPERTIES =============================
        public PlayerCharacter CurrentPC => _contexts.TurnContext.CurrentPC;

        // ==== DEPENDENCIES SET VIA DEPENDENCY INJECTION IN THE CONSTRUCTOR ========================
        private ContextManager _contexts;
        private EncounterManager _encounterManager;

        public TurnManager(ContextManager contexts, EncounterManager encounterManager)
        {
            _contexts = contexts;
            _encounterManager = encounterManager;
        }

        public void StartTurn(PlayerCharacter pc, Deck locationDeck)
        {
            this.locationDeck = locationDeck;

            // Advance the hour - happens automatically.
            //var hourCard = hoursDeck.DrawCard();
            //hourDisplay.SetViewModel(CardViewModelFactory.CreateFrom(hourCard, _contexts.GameContext.AdventureNumber));
            //_contexts.NewTurn(new(hourCard, pc));

            // TODO: Apply start of turn effects.

            // Set initial availability of turn actions
            canGive = true; // TODO: Implement logic after we have multiple characters.
            canMove = true; // TODO: Implement logic after we have multiple locations
            canExplore = locationDeck.Count > 0;
            canCloseLocation = locationDeck.Count == 0;

            _contexts.EndTurn();
        }

        public void GiveCard()
        {
            // TODO: Implement giving a card after we have multiple characters.
            canGive = false;
        }

        public void MoveToLocation()
        {
            // TODO: Implement moving to a location after we have multiple locations.
            canGive = false;
            canMove = false;
        }

        public void Explore()
        {
            canGive = false;
            canMove = false;
            canExplore = false;
        }

        public void OptionalDiscards()
        {
            canGive = false;
            canMove = false;
            canExplore = false;
        }

        public void EndTurn()
        {

        }

        public void RunEncounter()
        {
            CardInstance exploredCard = locationDeck.DrawCard();

            if (exploredCard == null)
                return;

            // Show the encountered card in UI.
            GameEvents.RaiseEncounterStarted(exploredCard);

            //GameObject encounterObject = new($"Encounter_{exploredCard.Data.cardID}");

            _contexts.NewEncounter(new(_contexts.TurnContext.CurrentPC, exploredCard));

            _encounterManager.RunEncounter();

            Debug.Log("Encounter finished.");

            if (_contexts.EncounterContext.CheckResult?.WasSuccess ?? false)
            {
                if (exploredCard.Data is BoonCardData)
                {
                    // TODO: Handle boon acquisition.
                }
                else
                {
                    Debug.Log("Bane banished.");
                }
            }
            else
            {
                Debug.Log("Do damage.");
                // Do damage later.
            }
            _contexts.EndEncounter();

            //Destroy(encounterObject);
        }

        public void OnEndTurnClicked()
        {

        }
    }
}
