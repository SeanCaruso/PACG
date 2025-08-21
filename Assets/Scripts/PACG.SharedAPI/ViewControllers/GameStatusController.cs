using PACG.Presentation;
using TMPro;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class GameStatusController : MonoBehaviour
    {
        [Header("Game Area")]
        public TextMeshProUGUI StatusText;

        [Header("Character Area")]
        public GameObject DeckCountPanel;
        
        [Header("Prefabs")]
        public DicePreview DicePreviewPrefab;

        private DicePreview _currentDicePreview;

        public void Awake()
        {
            GameEvents.SetStatusTextEvent += SetStatusText;
            GameEvents.DicePoolChanged += OnDicePoolChanged;
            GameEvents.PlayerDeckCountChanged += OnPlayerDeckCountChanged;
        }

        public void OnDestroy()
        {
            GameEvents.SetStatusTextEvent -= SetStatusText;
            GameEvents.DicePoolChanged -= OnDicePoolChanged;
            GameEvents.PlayerDeckCountChanged -= OnPlayerDeckCountChanged;
        }

        private void SetStatusText(string text)
        {
            if (_currentDicePreview)
                _currentDicePreview.ClearDice();
            StatusText.text = text;
        }

        private void OnPlayerDeckCountChanged(int count)
        {
            DeckCountPanel.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
        }

        private void OnDicePoolChanged(DicePool pool)
        {
            if (_currentDicePreview)
                Destroy(_currentDicePreview.gameObject);
            
            _currentDicePreview = Instantiate(DicePreviewPrefab, StatusText.gameObject.transform);
            _currentDicePreview.DisplayDicePool(pool);
        }
    }
}
