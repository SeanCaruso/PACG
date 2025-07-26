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

    private readonly List<CardData> playerHand = new();

    public void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        // Set up the game context.
        ServiceLocator.Get<ContextManager>().GameContext = new(1);

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

        handController.AddCard(card);
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
        newCard.SetCardData(exploredCard);
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
        ContextManager contextManager = ServiceLocator.Get<ContextManager>();
        contextManager.TurnContext = new(hourBlessing, testCharacter);
        contextManager.EncounterContext = new(exploredCard, encounterManager);

        yield return encounterManager.RunEncounter();

        if (Game.EncounterContext.CheckResult?.WasSuccess ?? false)
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
}
