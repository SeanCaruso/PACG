using PACG.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PACG.SharedAPI
{
    public class DebugInputController : MonoBehaviour
    {
        private PlayerCharacter _pc;
        
        private ContextManager _contexts;

        public void Initialize(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        private void OnEnable()
        {
            GameEvents.PlayerCharacterChanged += SetPc;
        }
        
        private void OnDisable()
        {
            GameEvents.PlayerCharacterChanged -= SetPc;
        }
        
        private void SetPc(PlayerCharacter pc) => _pc = pc;

#if UNITY_EDITOR
        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            var ctrl = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
            var shift = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;

            if (ctrl && shift && keyboard.eKey.isPressed)
            {
                if (keyboard.digit1Key.wasPressedThisFrame)
                    ExamineLocation(1);
                if (keyboard.digit2Key.wasPressedThisFrame)
                    ExamineLocation(2);
                if (keyboard.digit3Key.wasPressedThisFrame)
                    ExamineLocation(3);
            }

            if (ctrl && shift && keyboard.dKey.wasPressedThisFrame)
            {
                if (_pc.Deck.Count == 0) return;
                _pc.AddToHand(_pc.DrawFromDeck());
                Debug.Log($"NEW HAND: {string.Join(",", _pc.Hand)}");
            }

            if (ctrl && shift && keyboard.rKey.wasPressedThisFrame)
            {
                var hand = _pc.Hand;
                if (hand.Count == 0) return;
                _pc.Recharge(hand[^1]);
                Debug.Log($"NEW HAND: {string.Join(",", _pc.Hand)}");
            }

            if (ctrl && shift && keyboard.deleteKey.wasPressedThisFrame)
            {
                var hand = _pc.Hand;
                if (hand.Count == 0) return;
                _pc.Discard(hand[^1]);
                GameEvents.RaiseTurnStateChanged();
            }

            if (ctrl && shift && keyboard.hKey.wasPressedThisFrame)
            {
                if (_pc.Discards.Count == 0) return;
                _pc.Heal(1);
                GameEvents.RaiseTurnStateChanged();
            }
        }
#endif

        private void ExamineLocation(int count)
        {
            var loc = _pc.Location;
            Debug.Log($"[{GetType().Name}] Examining {count} from {loc}");

            var resolvable = new ExamineResolvable(loc, count, Keyboard.current.leftAltKey.isPressed);
            _contexts.NewResolvable(resolvable);
        }
    }
}
