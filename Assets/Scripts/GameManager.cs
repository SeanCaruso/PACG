using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
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

    private CardData currentEncounterCard;

    private void Start()
    {
        playerDeck.Shuffle();
        locationDeck.Shuffle();

        SetupInitialHand();
    }

    void SetupInitialHand()
    {
        for (int i = 0; i < startingHandSize; ++i)
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

        GameObject encounterObject = new GameObject($"Encounter_{exploredCard.cardID}");
        EncounterManager encounterManager = encounterObject.AddComponent<EncounterManager>();

        PlayerCharacter testCharacter = new();
        testCharacter.deck = playerDeck;
        testCharacter.hand = playerHand;
        testCharacter.proficiencies.Add(PF.CardType.Weapon);
        EncounterContext context = new EncounterContext(exploredCard, testCharacter, encounterManager);

        yield return encounterManager.RunEncounter(context);

        if (context.CheckResult?.WasSuccess ?? false)
        {
            Debug.Log("Success!");
            if (currentEncounterCard is BoonCardData)
            {
                CreateCardInHand(currentEncounterCard);
            }
        }
        else
        {
            Debug.Log("Failure!");
            // Do damage later.
        }

        Destroy(encounterObject);
    }
}
