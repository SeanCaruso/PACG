using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
    public CharacterData characterData;
    public List<CardData> playerDeck;
    public List<CardData> locationCards;

    [Header("UI References")]
    public UIInputController uIInputController;
    public TextMeshProUGUI checkButtonText;
    public Transform handHolder;
    public DiceRollUI diceRollUI;

    [Header("Game Rules")]
    public int startingHandSize = 5;

    public PlayerCharacter testCharacter = null;

    public void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        // Set up test data
        testCharacter = new()
        {
            characterData = characterData
        };
        foreach (var card in playerDeck) testCharacter.ShuffleIntoDeck(Game.CardManager.New(card, testCharacter));
        Deck locationDeck = new();
        foreach (var card in locationCards) locationDeck.ShuffleIn(Game.CardManager.New(card));
        locationDeck.Shuffle();
        ServiceLocator.Get<PlayerHandController>().SetCurrentPC(testCharacter);

        // Set up the game context.
        ServiceLocator.Get<ContextManager>().NewGame(new(1));
        testCharacter.DrawToHandSize();

        StartCoroutine(ServiceLocator.Get<TurnManager>().StartTurn(testCharacter, locationDeck));


        // Initialize UI if not already done.
        if (!uIInputController)
            uIInputController = FindFirstObjectByType<UIInputController>();
    }
}
