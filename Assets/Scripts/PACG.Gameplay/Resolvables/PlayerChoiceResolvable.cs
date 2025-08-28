using System;
using System.Collections.Generic;
using System.Linq;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class PlayerChoiceResolvable : BaseResolvable
    {
        public class ChoiceOption
        {
            public string Label { get; }
            public Action Action { get; }

            public ChoiceOption(string label, Action action)
            {
                Label = label;
                Action = action;
            }
        }

        public string Prompt { get; }
        public IReadOnlyList<ChoiceOption> Options { get; }

        public PlayerChoiceResolvable(string prompt, params ChoiceOption[] options)
        {
            Prompt = prompt;
            Options = options.ToList();
        }

        public override void Initialize()
        {
            GameEvents.RaisePlayerChoiceEvent(this);
        }

        public override StagedActionsState GetUIState(List<IStagedAction> _)
        {
            // This handles its own buttons.
            return new StagedActionsState();
        }
    }
}
