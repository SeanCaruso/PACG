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

        GameServices _gameServices;

        private void Awake()
        {
            // =================================================================
            // STEP 1: CONSTRUCT ALL PURE C# SERVICES AND PROCESSORS
            // =================================================================
            // These are just normal C# objects, no Unity dependency.
            var asm = new ActionStagingManager();
            var cardManager = new CardManager();
            var contextManager = new ContextManager();
            var gameFlowManager = new GameFlowManager();
            var logicRegistry = new LogicRegistry();

            _gameServices = new(
                asm,
                cardManager,
                contextManager,
                gameFlowManager,
                logicRegistry);

            // Initialize now that GameServices is set up.
            asm.Iniitalize(_gameServices);
            cardManager.Initialize(_gameServices);
            contextManager.Iniitalize(_gameServices);
            gameFlowManager.Initialize(_gameServices);
            logicRegistry.Initialize(_gameServices);
        }

        // Start is called after all Awake() methods are finished.
        private void Start()
        {
            _gameServices.Contexts.NewGame(new(1));
            CardUtils.Initialize(_gameServices.Contexts.GameContext.AdventureNumber);
            // =================================================================
            // STEP 3: WIRE UP THE PRESENTATION LAYER
            // The presentation layer also needs to know about certain services.
            // We can create an Initialize method for them.
            // =================================================================

            cardPreviewController.Initialize(_gameServices);
            uIInputController.Initialize(_gameServices);

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
                _gameServices.Contexts.GameContext.HourDeck.ShuffleIn(_gameServices.Cards.New(hourCardData));
            }

            Deck locationDeck = new();
            foreach (var cardData in testLocation)
            {
                locationDeck.ShuffleIn(_gameServices.Cards.New(cardData));
            }

            PlayerCharacter testPc = new(testCharacter, _gameServices.Cards);
            foreach (var card in characterDeck) testPc.ShuffleIntoDeck(_gameServices.Cards.New(card, testPc));
            cardDisplayController.SetCurrentPC(testPc);

            testPc.DrawToHandSize();
            var turnProcessor = new StartTurnProcessor(testPc, locationDeck, _gameServices);
            _gameServices.GameFlow.StartPhase(turnProcessor);
        }
    }
}
