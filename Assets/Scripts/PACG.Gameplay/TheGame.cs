using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using PACG.Data;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    [System.Serializable]
    public class TestCharacter
    {
        public CharacterData Pc;
        public List<CardData> Deck;
    }

    public class TheGame : MonoBehaviour
    {
        public CardPreviewController CardPreviewController;
        public DeckExamineController DeckExamineController;
        public PlayerChoiceController PlayerChoiceController;
        public UIInputController UiInputController;

        [Header("Debug Controller")]
        public DebugInputController DebugController;

        // === TEMPORARY MEMBERS FOR DEVELOPMENT ==========================================
        // As we add features, these should be removed.
        [Header("Test Objects")]
        public int AdventureNumber;
        public ScenarioData ScenarioData;
        public CardData hourCardData;
        public List<TestCharacter> TestCharacters;
        public List<LocationData> TestLocations;
        public List<CardData> testLocationDeck;

        public List<string> CharactersToUse;

        // Valeros: Half-plate, Light Shield, Longbow, Longsword, Longspear, Throwing Axe, Helm, Crowbar, Spyglass,
        //          Prayer, Prayer, Prayer, Soldier, Horse, Lookout
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
            CardUtils.Initialize(AdventureNumber);
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
            LeanTween.reset();

            var scenarioData = Instantiate(ScenarioData);
            _gameServices.Contexts.NewGame(new GameContext(AdventureNumber, scenarioData, _gameServices));

            if (!string.IsNullOrEmpty(scenarioData.DuringScenario))
                GameEvents.RaiseScenarioHasPower(_gameServices);

            // Set up test data
            for (var i = 0; i < 30; i++)
            {
                _gameServices.Contexts.GameContext.HourDeck.ShuffleIn(_gameServices.Cards.New(hourCardData));
            }

            foreach (var location in from testLocation in TestLocations
                     let locationLogic = _gameServices.Logic.GetLogic<LocationLogicBase>(testLocation.LocationName)
                     select new Location(testLocation, locationLogic, _gameServices))
            {
                _gameServices.Contexts.GameContext.AddLocation(location);

                foreach (var cardData in testLocationDeck)
                {
                    location.ShuffleIn(_gameServices.Cards.New(cardData), true);
                    location.ShuffleIn(_gameServices.Cards.New(scenarioData.Henchmen[0].CardData), false);
                }
            }

            foreach (var character in TestCharacters)
            {
                if (!CharactersToUse.Contains(character.Pc.CharacterName)) continue;

                var pcLogic = _gameServices.Logic.GetLogic<CharacterLogicBase>(character.Pc.CharacterName);
                PlayerCharacter testPc = new(character.Pc, pcLogic, _gameServices);

                foreach (var card in character.Deck) testPc.ShuffleIntoDeck(_gameServices.Cards.New(card, testPc));

                _gameServices.Contexts.GameContext.SetPcLocation(
                    testPc,
                    _gameServices.Contexts.GameContext.Locations.First()
                );

                testPc.DrawInitialHand();
            }

            var firstPc = _gameServices.Contexts.GameContext.Characters.First();
            GameEvents.RaisePlayerCharacterChanged(firstPc);
            var turnController = new StartTurnController(firstPc, _gameServices);
            _gameServices.GameFlow.StartPhase(turnController, "Turn");
        }
    }
}
