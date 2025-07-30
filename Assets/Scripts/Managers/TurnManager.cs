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
    public Deck HoursDeck;
    public CardData TestHourData;
    public CardDisplay HourDisplay;

    [Header("Turn Phase Buttons")]
    public Button GiveCardButton;
    public Button MoveButton;
    public Button ExploreButton;
    public Button ResetHandButton;
    public Button EndTurnButton;

    public void Awake()
    {
        ServiceLocator.Register(this);

        for (int i = 0; i < 30;  i++)
        {
            HoursDeck.Recharge(new(TestHourData));
        }
    }

    public IEnumerator StartTurn(PlayerCharacter pc)
    {
        var hourCard = HoursDeck.DrawCard();
        HourDisplay.SetCardInstance(hourCard);
        ServiceLocator.Get<ContextManager>().NewTurn(new(hourCard, pc));

        // TODO: Implement giving a card.
        GiveCardButton.enabled = false;

        yield break;
    }
}
