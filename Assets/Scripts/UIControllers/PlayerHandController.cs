using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    [Header("Hand Layout")]
    public RectTransform handContainer;
    public float maxHandWidth = 1200f;
    public float cardSpacing = 120f;
    public float fanRadius = 600f;
    public float hoverHeight = 40f;
    public AnimationCurve fanCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Card Management")]
    public CardDisplay cardPrefab;

    private PlayerCharacter _pc = null;
    private readonly List<CardDisplay> cardsInHand = new();

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void OnDestroy()
    {
        
    }

    public void SetCurrentPC(PlayerCharacter pc)
    {
        if (_pc != null) _pc.HandChanged -= OnHandChanged;
        _pc = pc;
        _pc.HandChanged += OnHandChanged;
    }

    private void OnHandChanged()
    {
        cardsInHand.Clear();
        foreach (var card in _pc.Hand) AddCard(card);
    }

    public void AddCard(CardInstance card)
    {
        CardDisplay newCard = Instantiate(cardPrefab, handContainer);
        newCard.SetCardInstance(card);
        newCard.transform.SetParent(handContainer);
        newCard.transform.localScale = Vector3.one;
        newCard.isInHand = true;

        cardsInHand.Add(newCard);
        //ArrangeCards();

        // Animate card draw.
        //AnimateCardDraw(newCard);
    }

    //public void RemoveCard(CardDisplay card)
    //{
    //    if (cardsInHand.Remove(card))
    //    {
    //        card.isInHand = false;
    //        ArrangeCards();
    //    }
    //}

    void ArrangeCards()
    {
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return;

        float totalWidth = Mathf.Min(cardCount * cardSpacing, maxHandWidth);
        float startX = -totalWidth * 0.5f;

        for (int i = 0; i < cardCount; i++)
        {
            float normalizedPos = cardCount > 1 ? (float)i / (cardCount - 1) : 0.5f;
            float xPos = startX + (normalizedPos * totalWidth);

            // Apply fan curve for natural card spread.
            float curveValue = fanCurve.Evaluate(normalizedPos);
            float yOffset = curveValue * hoverHeight;
            float rotation = (normalizedPos - 0.5f) * 20f; // Max 20 degree rotation

            Vector3 targetPos = new(xPos, yOffset, 0);
            Vector3 targetRot = new(0, 0, rotation);

            // Smooth animation to new position
            StartCoroutine(AnimateCardToPosition(cardsInHand[i], targetPos, targetRot));
        }
    }

    IEnumerator AnimateCardToPosition(CardDisplay card, Vector3 targetPos, Vector3 targetRot)
    {
        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 startPos = card.transform.localPosition;
        Vector3 startRot = card.transform.localEulerAngles;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            card.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            card.transform.localEulerAngles = Vector3.Lerp(startRot, targetRot, t);

            yield return null;
        }
    }

    void AnimateCardDraw(CardDisplay card)
    {
        // Start from deck position, animate to hand.
        Vector3 deckPos = FindFirstObjectByType<DeckController>().transform.position;
        card.transform.position = deckPos;
        card.transform.localScale = Vector3.zero;

        // Animate scale and position.
        //LeanTween.scale(card.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
    }
}
