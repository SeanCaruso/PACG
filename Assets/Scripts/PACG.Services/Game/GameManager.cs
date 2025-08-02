using PACG.Core.Characters;
using PACG.Presentation.UI.Controllers;
using PACG.Services.Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : GameBehaviour
{
    [Header("Object References")]
    public CharacterData characterData;
    public List<CardData> playerDeck;
    public List<CardData> locationCards;

    [Header("UI References")]
    public UIInputController uIInputController;
    public TextMeshProUGUI checkButtonText;
    public Transform handHolder;

    [Header("Game Rules")]
    public int startingHandSize = 5;

    public PlayerCharacter testCharacter = null;

    private void Start()
    {
        // Set up test data
        testCharacter = new(characterData, Cards);
        foreach (var card in playerDeck) testCharacter.ShuffleIntoDeck(Cards.New(card, testCharacter));
        Deck locationDeck = new();
        foreach (var card in locationCards) locationDeck.ShuffleIn(Cards.New(card));
        locationDeck.Shuffle();
        ServiceLocator.Get<CardDisplayController>().SetCurrentPC(testCharacter);

        // Set up the game context.
        ServiceLocator.Get<ContextManager>().NewGame(new(1));
        testCharacter.DrawToHandSize();

        StartCoroutine(ServiceLocator.Get<TurnManager>().StartTurn(testCharacter, locationDeck));


        // Initialize UI if not already done.
        if (!uIInputController)
            uIInputController = FindFirstObjectByType<UIInputController>();
    }
}
