using PACG.Core.Characters;
using PACG.Presentation.Cards;
using PACG.Services.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Services.Game
{
    public class TurnManager : GameBehaviour
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

        // ==== TURN PHASE AVAILABILITY ============================
        private bool canGive = false;
        public bool CanGive => canGive;

        private bool canMove = false;
        public bool CanMove => canMove;

        private bool canExplore = false;
        public bool CanExplore => canExplore;

        private bool canCloseLocation = false;
        public bool CanCloseLocation => canCloseLocation;

        private bool canEndTurn = false;
        public bool CanEndTurn => canEndTurn;

        public void Start()
        {
            for (int i = 0; i < 30; i++)
            {
                hoursDeck.Recharge(Cards.New(testHourData));
            }
        }

        public void StartTurn(PlayerCharacter pc, Deck locationDeck)
        {
            this.locationDeck = locationDeck;

            // Advance the hour - happens automatically.
            var hourCard = hoursDeck.DrawCard();
            hourDisplay.SetCardInstance(hourCard);
            Contexts.NewTurn(new(hourCard, pc));

            // TODO: Apply start of turn effects.

            // Set initial availability of turn actions
            canGive = true; // TODO: Implement logic after we have multiple characters.
            canMove = true; // TODO: Implement logic after we have multiple locations
            canExplore = locationDeck.Count > 0;
            canCloseLocation = locationDeck.Count == 0;

            Contexts.EndTurn();
        }

        public void GiveCard()
        {
            // TODO: Implement giving a card after we have multiple characters.
            canGive = false;
        }

        public void MoveToLocation()
        {
            // TODO: Implement moving to a location after we have multiple locations.
            canGive = false;
            canMove = false;
        }

        public void Explore()
        {
            canGive = false;
            canMove = false;
            canExplore = false;
        }

        public void OptionalDiscards()
        {
            canGive = false;
            canMove = false;
            canExplore = false;
        }

        public void EndTurn()
        {

        }

        private void RunEncounter()
        {
            CardInstance exploredCard = locationDeck.DrawCard();

            if (exploredCard == null)
                return;

            // Show the encountered card in UI.
            CardDisplay newCard = Instantiate(cardPrefab, encounterZone.transform);
            newCard.SetCardInstance(exploredCard);
            newCard.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            newCard.transform.localScale = Vector3.one;

            //GameObject encounterObject = new($"Encounter_{exploredCard.Data.cardID}");
            EncounterManager encounterManager = ServiceLocator.Get<EncounterManager>(); //encounterObject.AddComponent<EncounterManager>();

            Contexts.NewEncounter(new(Contexts.TurnContext.CurrentPC, exploredCard));

            encounterManager.RunEncounter();

            Debug.Log("Encounter finished.");

            if (Contexts.EncounterContext.CheckResult?.WasSuccess ?? false)
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
            Contexts.EndEncounter();

            Destroy(encounterObject);
        }

        public void OnEndTurnClicked()
        {

        }
    }
}
