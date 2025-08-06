using PACG.SharedAPI;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class TheGame : MonoBehaviour
    {
        [Header("Shared API Controllers")]
        public CardDisplayController cardDisplayController;
        public CardPreviewController cardPreviewController;
        public UIInputController uIInputController;

        // === TEMPORARY MEMBERS FOR DEVELOPMENT ==========================================
        // As we add features, these should be removed.
        [Header("Test Objects")]
        public CardData hourCardData;
        public CharacterData testCharacter;
        public List<CardData> characterDeck;
        public List<CardData> testLocation;
        // ================================================================================

        ContextManager _contextManager;
        LogicRegistry _logicRegistry;
        CardManager _tempCardManager;
        TurnManager _turnManager;
        ActionStagingManager _asm;

        private void Awake()
        {
            // =================================================================
            // STEP 1: CONSTRUCT ALL PURE C# SERVICES AND PROCESSORS
            // =================================================================
            // These are just normal C# objects, no Unity dependency.
            _contextManager = new ContextManager();
            var cardManager = new CardManager();

            _logicRegistry = new LogicRegistry(_contextManager);
            _asm = new ActionStagingManager(_contextManager, cardManager);
            var encounterManager = new EncounterManager(_logicRegistry, _contextManager, _asm);
            _turnManager = new(_contextManager, encounterManager);

            _tempCardManager = cardManager;
        }

        // Start is called after all Awake() methods are finished.
        private void Start()
        {
            _contextManager.NewGame(new(1));
            // =================================================================
            // STEP 3: WIRE UP THE PRESENTATION LAYER
            // The presentation layer also needs to know about certain services.
            // We can create an Initialize method for them.
            // =================================================================

            cardDisplayController.Initialize(_contextManager.GameContext.AdventureNumber);
            cardPreviewController.Initialize(_logicRegistry, _asm);
            uIInputController.Initialize(_turnManager);

            // The PlayerCardsController needs a way to create ViewModels.

            // TODO: This is currently a static class... is that right?
            //var viewModelFactory = new CardViewModelFactory();
            //_playerCardsController.Initialize(viewModelFactory, _gameFlowManager); // Pass it the GFM to make requests

            // =================================================================
            // STEP 4: PRESS THE "ON" BUTTON
            // Everything is built and wired. Now we tell the engine to start.
            // =================================================================

            // Set up test data
            for (var i = 0; i < 30;  i++)
            {
                _contextManager.GameContext.HourDeck.ShuffleIn(_tempCardManager.New(hourCardData));
            }

            Deck locationDeck = new();
            foreach (var cardData in testLocation)
            {
                locationDeck.ShuffleIn(_tempCardManager.New(cardData));
            }

            PlayerCharacter testPc = new(testCharacter, _tempCardManager);
            foreach (var card in characterDeck) testPc.ShuffleIntoDeck(_tempCardManager.New(card, testPc));
            cardDisplayController.SetCurrentPC(testPc);

            testPc.DrawToHandSize();
            _turnManager.StartTurn(testPc, locationDeck);
        }
    }
}
