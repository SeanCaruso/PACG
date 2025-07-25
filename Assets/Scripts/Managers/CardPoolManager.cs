using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CardPoolManager : MonoBehaviour
{
    [Header("Pool Settings")]
    public CardDisplay cardPrefab;
    public int initialPoolSize = 30;
    public int maxPoolSize = 100;

    private Queue<CardDisplay> cardPool = new();
    private List<CardDisplay> activeCards = new();

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewCard();
        }
    }

    CardDisplay CreateNewCard()
    {
        GameObject cardObj = Instantiate(cardPrefab.gameObject);
        CardDisplay card = cardObj.GetComponent<CardDisplay>();

        // Disable raycast on non-interactive elements
        OptimizeCardPerformance(card);

        cardObj.SetActive(false);
        cardPool.Enqueue(card);

        return card;
    }

    void OptimizeCardPerformance(CardDisplay card)
    {
        // Disable raycast on decorative elements.
        Image[] images = card.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] texts = card.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var image in images) if (image.GetComponent<Button>() == null) image.raycastTarget = false;
        foreach (var text in texts) text.raycastTarget = false;
    }

    public CardDisplay GetCard()
    {
        CardDisplay card;
        if (cardPool.Count > 0)
        {
            card = cardPool.Dequeue();
        }
        else if (activeCards.Count < maxPoolSize)
        {
            card = CreateNewCard();
            cardPool.Dequeue(); // Remove from pool since we're using it.
        }
        else
        {
            Debug.LogWarning("Card pool exhausted! Consider increasing pool size.");
            return null;
        }

        card.gameObject.SetActive(true);
        activeCards.Add(card);

        return card;
    }

    public void ReturnCard(CardDisplay card)
    {
        if (activeCards.Remove(card))
        {
            // Reset card state
            card.transform.localScale = Vector3.one;
            card.transform.rotation = Quaternion.identity;
            card.isStaged = false;
            card.isInHand = false;
            card.isExpanded = false;

            card.gameObject.SetActive(false);
            cardPool.Enqueue(card);
        }
    }

    private void Update()
    {
        // Monitor pool usage for debugging.
        var keyboard = Keyboard.current;
        if (keyboard is null) return;

        if (keyboard[Key.P].wasPressedThisFrame)
        {
            Debug.Log($"Pool: {cardPool.Count} available, {activeCards.Count} active.");
        }
    }
}
