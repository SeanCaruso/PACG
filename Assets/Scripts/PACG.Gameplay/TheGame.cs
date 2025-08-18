using PACG.Data;
using PACG.SharedAPI;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class TheGame : MonoBehaviour
    {
        public CardDisplayController CardDisplayController;
        public CardPreviewController CardPreviewController;
        public DeckExamineController DeckExamineController;
        public PlayerChoiceController PlayerChoiceController;
        public UIInputController UiInputController;

        [Header("Debug Controller")]
        public DebugInputController DebugController;

        // === TEMPORARY MEMBERS FOR DEVELOPMENT ==========================================
        // As we add features, these should be removed.
        [Header("Test Objects")]
        public CardData hourCardData;
        public CharacterData testCharacter;
        public List<CardData> characterDeck;
        public LocationData testLocation;
        public List<CardData> testLocationDeck;
        // ================================================================================

        private GameServices _gameServices;

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

            _gameServices = new GameServices(
                asm,
                cardManager,
                contextManager,
                gameFlowManager,
                logicRegistry);

            // Initialize now that GameServices is set up.
            asm.Initialize(_gameServices);
            cardManager.Initialize(_gameServices);
            contextManager.Initialize(_gameServices);
            gameFlowManager.Initialize(_gameServices);
            logicRegistry.Initialize(_gameServices);
        }

        // Start is called after all Awake() methods are finished.
        private void Start()
        {
            _gameServices.Contexts.NewGame(new GameContext(1, _gameServices.Cards));
            CardUtils.Initialize(_gameServices.Contexts.GameContext.AdventureNumber);
            // =================================================================
            // STEP 3: WIRE UP THE PRESENTATION LAYER
            // The presentation layer also needs to know about certain services.
            // We can create an Initialize method for them.
            // =================================================================

            CardPreviewController.Initialize(_gameServices);
            DeckExamineController.Initialize(_gameServices);
            PlayerChoiceController.Initialize(_gameServices);
            UiInputController.Initialize(_gameServices);

            DebugController.Initialize(_gameServices);

            // =================================================================
            // STEP 4: PRESS THE "ON" BUTTON
            // Everything is built and wired. Now we tell the engine to start.
            // =================================================================

            // Set up test data
            for (var i = 0; i < 30;  i++)
            {
                _gameServices.Contexts.GameContext.HourDeck.ShuffleIn(_gameServices.Cards.New(hourCardData));
            }

            var locationLogic = _gameServices.Logic.GetLogic<LocationLogicBase>(testLocation.LocationName);
            Location location = new(testLocation, locationLogic, _gameServices);

            foreach (var cardData in testLocationDeck)
            {
                location.ShuffleIn(_gameServices.Cards.New(cardData), true);
            }

            var pcLogic = _gameServices.Logic.GetLogic<CharacterLogicBase>(testCharacter.characterName);
            PlayerCharacter testPc = new(testCharacter, pcLogic, _gameServices);
            foreach (var card in characterDeck) testPc.ShuffleIntoDeck(_gameServices.Cards.New(card, testPc));
            CardDisplayController.SetCurrentPC(testPc);

            _gameServices.Contexts.GameContext.SetPcLocation(testPc, location);

            testPc.DrawInitialHand();

            var turnController = new StartTurnController(testPc, _gameServices);
            _gameServices.GameFlow.StartPhase(turnController, "Turn");
        }
    }
}
