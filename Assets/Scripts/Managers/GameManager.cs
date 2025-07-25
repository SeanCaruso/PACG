using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
    public CharacterData characterData;
    public Deck playerDeck;
    public Deck locationDeck;
    public CardData hourBlessing;

    [Header("UI References")]
    public UIInputController uIInputController;
    public GameObject encounterZone;
    public TextMeshProUGUI checkButtonText;
    public Transform handHolder;
    public DiceRollUI diceRollUI;

    [Header("Prefab References")]
    public CardDisplay cardPrefab;

    [Header("Game Rules")]
    public int startingHandSize = 5;

    private List<CardData> playerHand = new();

    private GameContext gameContext = new(1);

    public void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        playerDeck.Shuffle();
        locationDeck.Shuffle();

        SetupInitialHand();

        // Initialize UI if not already done.
        if (!uIInputController)
            uIInputController = FindFirstObjectByType<UIInputController>();
    }

    void SetupInitialHand()
    {
        for (int i = 0; i < characterData.handSize; ++i)
        {
            CardData drawnCard = playerDeck.DrawCard();
            if (!drawnCard)
                return;

            CreateCardInHand(drawnCard);
        }
    }

    void CreateCardInHand(CardData card)
    {
        playerHand.Add(card);
        PlayerHandController handController = ServiceLocator.Get<PlayerHandController>();
        if (handController == null) Debug.LogError("Unable to find PlayerHandController!");

        handController.AddCard(card, gameContext);
    }

    public void OnExploreClicked()
    {
        StartCoroutine(RunEncounter());
    }

    private IEnumerator RunEncounter()
    {
        CardData exploredCard = locationDeck.DrawCard();

        if (!exploredCard)
            yield break;

        // Show the encountered card in UI.
        CardDisplay newCard = Instantiate(cardPrefab, encounterZone.transform);
        newCard.SetCardData(exploredCard, gameContext);
        newCard.transform.localScale = Vector3.one;
        newCard.transform.localPosition = Vector3.zero;

        GameObject encounterObject = new($"Encounter_{exploredCard.cardID}");
        EncounterManager encounterManager = encounterObject.AddComponent<EncounterManager>();

        PlayerCharacter testCharacter = new()
        {
            characterData = this.characterData,
            deck = playerDeck,
            hand = playerHand
        };
        TurnContext turnContext = new(gameContext, hourBlessing, testCharacter);
        EncounterContext context = new(turnContext, exploredCard, encounterManager);

        yield return encounterManager.RunEncounter(context);

        if (context.CheckResult?.WasSuccess ?? false)
        {
            if (exploredCard is BoonCardData)
            {
                CreateCardInHand(exploredCard);
            }
        }
        else
        {
            // Do damage later.
        }

        Destroy(encounterObject);
    }

    public void RefreshHandDisplay()
    {
        // Clear existing hand display
        foreach (Transform child in handHolder)
        {
            Destroy(child.gameObject);
        }

        // Recreate hand display.
        foreach (CardData card in playerHand)
        {
            CardDisplay newCard = Instantiate(cardPrefab, handHolder);
            newCard.SetCardData(card, gameContext);
        }
    }
}
