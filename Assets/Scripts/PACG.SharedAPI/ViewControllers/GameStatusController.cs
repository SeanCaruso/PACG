using TMPro;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class GameStatusController : MonoBehaviour
    {
        [Header("Game Area")]
        public TextMeshProUGUI statusText;

        [Header("Character Area")]
        public GameObject deckCountPanel;

        public void Awake()
        {
            GameEvents.SetStatusTextEvent += SetStatusText;
            GameEvents.PlayerDeckCountChanged += OnPlayerDeckCountChanged;
        }

        public void OnDestroy()
        {
            GameEvents.SetStatusTextEvent -= SetStatusText;
            GameEvents.PlayerDeckCountChanged -= OnPlayerDeckCountChanged;
        }

        private void SetStatusText(string text)
        {
            statusText.text = text;
        }

        private void OnPlayerDeckCountChanged(int count)
        {
            deckCountPanel.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
        }
    }
}
