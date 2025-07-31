using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TurnPhase
{
    AdvanceHour,
    GiveCard,
    Move,
    Explore,
    CloseLocation,
    EndTurn
}

public class TurnManager : MonoBehaviour
{
    [Header("The Hour")]
    public CardData testHourData;
    public CardDisplay hourDisplay;

    [Header("UI References")]
    public GameObject encounterZone;

    [Header("Prefab References")]
    public CardDisplay cardPrefab;

    private readonly Deck hoursDeck = new();
    private Deck locationDeck = new();

    public void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            hoursDeck.Recharge(ServiceLocator.Get<CardManager>().New(testHourData));
        }
    }

    public IEnumerator StartTurn(PlayerCharacter pc, Deck locationDeck)
    {
        this.locationDeck = locationDeck;

        // Advance the hour - happens automatically.
        var hourCard = hoursDeck.DrawCard();
        hourDisplay.SetCardInstance(hourCard);
        Game.NewTurn(new(hourCard, pc));

        // TODO: Apply start of turn effects.

        // Set initial state of turn phase buttons.
        //giveCardButton.enabled = true; // TODO: Test for local characters
        //moveButton.enabled = true; // TODO: Implement when we have multiple locations
        //locationDeckButton.GetComponent<Button>().enabled = locationDeck.Count > 0;
        //resetHandButton.enabled = true;
        //endTurnButton.enabled = true;

        Game.EndTurn();

        yield break;
    }

    public void OnGiveCardClicked()
    {
        StartCoroutine(RunGiveCard());
    }

    private IEnumerator RunGiveCard()
    {
        // TODO: Implement giving a card.
        Game.NewResolution(new(new GiveCardResolvable(null)));
        yield return Game.ResolutionContext.WaitForResolution();
        Game.EndResolution();
    }

    public void OnExploreClicked()
    {
        StartCoroutine(RunEncounter());
    }

    private IEnumerator RunEncounter()
    {
        CardInstance exploredCard = locationDeck.DrawCard();

        if (exploredCard == null)
            yield break;

        // Show the encountered card in UI.
        CardDisplay newCard = Instantiate(cardPrefab, encounterZone.transform);
        newCard.SetCardInstance(exploredCard);
        newCard.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        newCard.transform.localScale = Vector3.one;

        GameObject encounterObject = new($"Encounter_{exploredCard.Data.cardID}");
        EncounterManager encounterManager = encounterObject.AddComponent<EncounterManager>();

        Game.NewEncounter(new(Game.TurnContext.CurrentPC, exploredCard));

        yield return encounterManager.RunEncounter();

        Debug.Log("Encounter finished.");

        if (Game.EncounterContext.CheckResult?.WasSuccess ?? false)
        {
            if (exploredCard.Data is BoonCardData)
            {
                // TODO: Handle boon acquisition.
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

    public void OnEndTurnClicked()
    {

    }
}
