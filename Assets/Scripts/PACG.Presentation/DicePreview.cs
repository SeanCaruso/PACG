using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation
{
    public class DicePreview : MonoBehaviour
    {
        [Header("Sprites")]
        public Sprite D4Sprite;
        public Sprite D6Sprite;
        public Sprite D8Sprite;
        public Sprite D10Sprite;
        public Sprite D12Sprite;
        public Sprite D20Sprite;

        public void DisplayDicePool(DicePool dicePool)
        {
            ClearDice();

            // Create child objects for each die type
            CreateDiceImages(dicePool.NumDice(20), D20Sprite, "d20");
            CreateDiceImages(dicePool.NumDice(12), D12Sprite, "d12");
            CreateDiceImages(dicePool.NumDice(10), D10Sprite, "d10");
            CreateDiceImages(dicePool.NumDice(8), D8Sprite, "d8");
            CreateDiceImages(dicePool.NumDice(6), D6Sprite, "d6");
            CreateDiceImages(dicePool.NumDice(4), D4Sprite, "d4");
        }

        public void ClearDice()
        {
            // Clear existing dice images
            for (var i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }

        private void CreateDiceImages(int count, Sprite dieSprite, string dieName)
        {
            for (var i = 0; i < count; i++)
            {
                var dieObject = new GameObject($"{dieName}_{i}");
                dieObject.transform.SetParent(transform, false);
                
                var image = dieObject.AddComponent<Image>();
                image.sprite = dieSprite;
                
                // Optional: Set size and layout properties
                var rectTransform = dieObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(64, 64); // Adjust size as needed
            }
        }
    }
}
