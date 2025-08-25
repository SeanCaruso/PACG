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
            return new PlayerCharacter(characterData, null, gameServices);
        }

        public static Location GetLocation(GameServices gameServices, string locationName)
        {
            var locationData = LoadCardData<LocationData>(locationName);
            return new Location(locationData, null, gameServices);
        }

        public static void SetupEncounter(GameServices gameServices, string character, string card)
        {
            var location = GetLocation(gameServices, "Caravan");
            var pc = GetCharacter(gameServices, character);
            var encounterCard = GetCard(gameServices, card);
        
            gameServices.Contexts.NewGame(new GameContext(1, gameServices.Cards));
            gameServices.Contexts.GameContext.SetPcLocation(pc, location);
        
            gameServices.Contexts.NewTurn(new TurnContext(pc));
            gameServices.GameFlow.StartPhase(new EncounterController(pc, encounterCard, gameServices), "Encounter");
        }
    }

    public class BaseTest
    {
        protected GameServices GameServices;

        [SetUp]
        public virtual void Setup()
        {
            GameServices = TestUtils.CreateGameServices();
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
