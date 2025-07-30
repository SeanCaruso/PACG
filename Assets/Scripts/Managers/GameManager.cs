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

    public PlayerCharacter testCharacter = null;

    public void Awake()
    {
        ServiceLocator.Register(this);
        testCharacter = new()
        {
            characterData = characterData,
            deck = playerDeck
        };
    }

    private void Start()
    {
        // Set up the game context.
        ServiceLocator.Get<ContextManager>().NewGame(new(1));
        ServiceLocator.Get<ContextManager>().NewTurn(new(hourBlessing, testCharacter));

        foreach (var card in playerDeck.cards)
        {
            card.OriginalOwner = testCharacter;
            card.Owner = testCharacter;
        }

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
        testCharacter.hand.Add(card);
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
        newCard.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        newCard.transform.localScale = Vector3.one;

        GameObject encounterObject = new($"Encounter_{exploredCard.cardID}");
        EncounterManager encounterManager = encounterObject.AddComponent<EncounterManager>();

        ContextManager contextManager = ServiceLocator.Get<ContextManager>();
        Game.NewTurn(new(hourBlessing, testCharacter));
        Game.NewEncounter(new(testCharacter, exploredCard));

        yield return encounterManager.RunEncounter();

        Debug.Log("Encounter finished.");

        if (Game.EncounterContext.CheckResult?.WasSuccess ?? false)
        {
            if (exploredCard is BoonCardData)
            {
                Debug.Log("Boon obtained.");
                CreateCardInHand(exploredCard);
                exploredCard.Owner = testCharacter;
                exploredCard.OriginalOwner ??= testCharacter;
            }
            else
            {
                Debug.Log("Bane banished.");
                Destroy(newCard.gameObject);
            }
        }
        else
        {
            Debug.Log("Do damage.");
            Destroy(newCard.gameObject);
            // Do damage later.
        }
        Game.EndEncounter();

        Destroy(encounterObject);
    }
}
