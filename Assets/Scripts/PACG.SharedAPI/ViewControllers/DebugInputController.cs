using PACG.Gameplay;
using System.Collections;
using System.Collections.Generic;
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
        void Update()
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
        }
#endif

        private void ExamineLocation(int count)
        {

            var loc = _contexts.TurnContext.Character.Location;
            Debug.Log($"[{GetType().Name}] Examining {count} from {loc}");

            var resolvable = new ExamineResolvable(loc, count, loc.Count, false);
            _contexts.NewResolvable(resolvable);
        }
    }
}
