using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation
{
    public class DeckExamineDialog : MonoBehaviour
    {
        [Header("UI Elements")] public GameObject BackgroundArea;
        public Transform CardBacksContainer;
        public Transform CardsContainer;
        public Transform ButtonContainer;
        
        [Header("Prefabs")] public Button ButtonPrefab;

        private void OnEnable()
        {
            ButtonPrefab.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
        }
    }
}
