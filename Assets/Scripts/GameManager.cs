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
        GameObject newCardObject = Instantiate(cardPrefab, handHolder);

        CardDisplay cardDisplay = newCardObject.GetComponent<CardDisplay>();

        cardDisplay.cardData = card;
        cardDisplay.UpdateCardDisplay();
    }

    public void OnExploreClicked()
    {
        CardData exploredCard = locationDeck.DrawCard();
        if (exploredCard)
            StartEncounter(exploredCard);
    }

    void StartEncounter(CardData card)
    {
        currentEncounterCard = card;

        encounterZone.SetActive(true);
        encounterCardDisplay.cardData = card;
        encounterCardDisplay.UpdateCardDisplay();

        checkButtonText.text = card is BaneCardData ? "Attempt to Defeat" : "Attempt to Acquire";
    }

    public void OnEncounterCheck()
    {
        int roll = Random.Range(1, 20);

        bool success = false;// roll >= currentEncounterCard.CheckDC;

        if (success)
        {
            Debug.Log("Player rolled a " + roll + "... Success!");
            if (currentEncounterCard is BoonCardData)
            {
                CreateCardInHand(currentEncounterCard);
            }
        }
        else
        {
            Debug.Log("Player rolled a " + roll + "... Failure!");
        }

        EndEncounter();
    }

    void EndEncounter()
    {
        encounterZone.SetActive(false);
        currentEncounterCard = null;
    }
}
