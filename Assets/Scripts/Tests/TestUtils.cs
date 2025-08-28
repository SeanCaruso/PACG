using NUnit.Framework;
using PACG.Data;
using PACG.Gameplay;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Tests
{
    public static class TestUtils
    {
        public static GameServices CreateGameServices()
        {
            CardUtils.Initialize(1);
        
            var asm = new ActionStagingManager();
            var cardManager = new CardManager();
            var contextManager = new ContextManager();
            var gameFlow = new GameFlowManager();
            var logicRegistry = new LogicRegistry();

            var gameServices = new GameServices(
                asm,
                cardManager,
                contextManager,
                gameFlow,
                logicRegistry);

            asm.Initialize(gameServices);
            cardManager.Initialize(gameServices);
            contextManager.Initialize(gameServices);
            gameFlow.Initialize(gameServices);
            logicRegistry.Initialize(gameServices);

            return gameServices;
        }

#if UNITY_EDITOR

        public static T LoadCardData<T>(string assetName) where T : ScriptableObject
        {
            // Search for the card by name in the CardData folder structure.
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)} {assetName}", new[] { "Assets/CardData" });

            switch (guids.Length)
            {
                case 0:
                    throw new System.Exception($"Could not find card data for {assetName}!");
                case > 1:
                    Debug.LogWarning($"Found multiple card data assets for {assetName}!");
                    break;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif

        public static CardInstance GetCard(GameServices gameServices, string cardName)
        {
            var cardData = LoadCardData<CardData>(cardName);
            return gameServices.Cards.New(cardData);
        }
    
        public static PlayerCharacter GetCharacter(GameServices gameServices, string characterName)
        {
            var characterData = LoadCardData<CharacterData>(characterName);
            var logic = gameServices.Logic.GetLogic<CharacterLogicBase>(characterName);
            return new PlayerCharacter(characterData, logic, gameServices);
        }

        public static Location GetLocation(GameServices gameServices, string locationName)
        {
            var locationData = LoadCardData<LocationData>(locationName);
            var logic = gameServices.Logic.GetLogic<LocationLogicBase>(locationName);
            return new Location(locationData, logic, gameServices);
        }

        public static void SetupEncounter(GameServices gameServices, string character, string card)
        {
            var pc = GetCharacter(gameServices, character);
            var encounterCard = GetCard(gameServices, card);
            
            SetupEncounter(gameServices, pc, encounterCard);
        }

        public static void SetupEncounter(GameServices gameServices, PlayerCharacter pc, CardInstance card)
        {
            var location = GetLocation(gameServices, "Caravan");
        
            gameServices.Contexts.NewGame(new GameContext(1, gameServices.Cards));
            gameServices.Contexts.GameContext.SetPcLocation(pc, location);
        
            gameServices.Contexts.NewTurn(new TurnContext(pc));
            gameServices.GameFlow.StartPhase(new EncounterController(pc, card, gameServices), "Encounter");
        }
    }

    public class BaseTest
    {
        protected GameServices GameServices;
        
        // Common cards.
        protected PlayerCharacter Valeros;
        
        protected CardInstance Longsword;
        protected CardInstance Zombie;

        [SetUp]
        public virtual void Setup()
        {
            GameServices = TestUtils.CreateGameServices();
            
            Valeros = TestUtils.GetCharacter(GameServices, "Valeros");
            
            Longsword = TestUtils.GetCard(GameServices, "Longsword");
            Zombie = TestUtils.GetCard(GameServices, "Zombie");
        }

        [TearDown]
        public virtual void TearDown()
        {
            GameServices.Contexts.EndCheck();
            GameServices.Contexts.EndResolvable();
            GameServices.Contexts.EndEncounter();
            GameServices.Contexts.EndTurn();
        }
    }
}
