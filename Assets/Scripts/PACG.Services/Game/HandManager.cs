using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handPanel;

    private List<GameObject> cardsInHand = new();

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        GameObject newCard = Instantiate(cardPrefab, handPanel);
        cardsInHand.Add(newCard);
    }
}
