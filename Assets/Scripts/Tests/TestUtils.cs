using System.Collections.Generic;
using PACG.Data;
using PACG.Gameplay;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

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
    public static CardData LoadCardData(string cardName)
    {
        // Search for the card by name in the CardData folder structure.
        var guids = AssetDatabase.FindAssets($"t:{nameof(CardData)} {cardName}", new[] { "Assets/CardData" });

        switch (guids.Length)
        {
            case 0:
                throw new System.Exception($"Could not find card data for {cardName}!");
            case > 1:
                Debug.LogWarning($"Found multiple card data assets for {cardName}!");
                break;
        }

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<CardData>(path);
    }
    
    public static CharacterData LoadCharacterData(string characterName)
    {
        // Search for the card by name in the CharacterData folder structure.
        var guids = AssetDatabase.FindAssets($"t:{nameof(CharacterData)} {characterName}", new[] { "Assets/CardData" });

        switch (guids.Length)
        {
            case 0:
                throw new System.Exception($"Could not find character data for {characterName}!");
            case > 1:
                Debug.LogWarning($"Found multiple character data assets for {characterName}!");
                break;
        }

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<CharacterData>(path);
    }
#endif

    public static void SetupCombatCheck(GameServices gameServices, PlayerCharacter pc, int dc = 10)
    {
        gameServices.Contexts.NewTurn(new TurnContext(pc));

        var encounterData = ScriptableObject.CreateInstance<BaneCardData>();
        encounterData.checkRequirement = new CheckRequirement
        {
            mode = CheckMode.Single
        };
        var checkSteps = new List<CheckStep>
        {
            new()
            {
                category = CheckCategory.Combat,
                baseDC = dc
            }
        };
        encounterData.checkRequirement.checkSteps = checkSteps;
        
        var encounterInstance = new CardInstance(encounterData, new ZombieLogic(gameServices));
        
        gameServices.Contexts.NewEncounter(new EncounterContext(pc, encounterInstance));
        gameServices.Contexts.NewResolvable(new CombatResolvable(encounterInstance, pc, dc));
    }

    public static void SetupEncounter(GameServices gameServices, string character, string card)
    {
        var characterData = LoadCharacterData(character);
        var cardData = LoadCardData(card);
        
        var pc = new PlayerCharacter(characterData, null, gameServices);
        var encounterCard = gameServices.Cards.New(cardData);
        
        gameServices.Contexts.NewTurn(new TurnContext(pc));
        gameServices.GameFlow.StartPhase(new EncounterController(pc, encounterCard, gameServices), "Encounter");
    }
}
