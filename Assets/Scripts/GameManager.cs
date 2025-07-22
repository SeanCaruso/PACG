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

    [Header("UI References")]
    public GameObject encounterZone;
    public CardDisplay encounterCardDisplay;
    public TextMeshProUGUI checkButtonText;
    public Transform handHolder;

    [Header("Prefab References")]
    public GameObject cardPrefab;

    [Header("Game Rules")]
    public int startingHandSize = 5;

    private List<CardData> playerHand = new();

    private GameContext gameContext = new(1);
    private CardData currentEncounterCard;

    private void Start()
    {
        playerDeck.Shuffle();
        locationDeck.Shuffle();

        SetupInitialHand();
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

        GameObject newCardObject = Instantiate(cardPrefab, handHolder);

        CardDisplay cardDisplay = newCardObject.GetComponent<CardDisplay>();

        cardDisplay.cardData = card;
        cardDisplay.UpdateCardDisplay();
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

        GameObject encounterObject = new($"Encounter_{exploredCard.cardID}");
        EncounterManager encounterManager = encounterObject.AddComponent<EncounterManager>();

        PlayerCharacter testCharacter = new()
        {
            characterData = this.characterData,
            deck = playerDeck,
            hand = playerHand
        };
        EncounterContext context = new(gameContext, exploredCard, testCharacter, encounterManager);

        yield return encounterManager.RunEncounter(context);

        if (context.CheckResult?.WasSuccess ?? false)
        {
            if (currentEncounterCard is BoonCardData)
            {
                CreateCardInHand(currentEncounterCard);
            }
        }
        else
        {
            // Do damage later.
        }

        Destroy(encounterObject);
    }
}
