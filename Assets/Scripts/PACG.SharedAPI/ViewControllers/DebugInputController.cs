using PACG.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PACG.SharedAPI
{
    public class DebugInputController : MonoBehaviour
    {
        private ContextManager _contexts;

        public void Initialize(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
        }

#if UNITY_EDITOR
        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            bool ctrl = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
            bool shift = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;

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
                var pc = _contexts.TurnContext.Character;
                pc.AddToHand(pc.DrawFromDeck());
                Debug.Log($"NEW HAND: {string.Join(",", _contexts.TurnContext.Character.Hand)}");
            }

            if (ctrl && shift && keyboard.rKey.wasPressedThisFrame)
            {
                var hand = _contexts.TurnContext.Character.Hand;
                _contexts.TurnContext.Character.Recharge(hand[^1]);
                Debug.Log($"NEW HAND: {string.Join(",", _contexts.TurnContext.Character.Hand)}");
            }
        }
#endif

        private void ExamineLocation(int count)
        {
            var loc = _contexts.TurnContext.Character.Location;
            Debug.Log($"[{GetType().Name}] Examining {count} from {loc}");

            var resolvable = new ExamineResolvable(loc, count, Keyboard.current.leftAltKey.isPressed);
            _contexts.NewResolvable(resolvable);
        }
    }
}
