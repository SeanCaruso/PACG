using PACG.SharedAPI;
using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class ExamineResolvable : BaseResolvable
    {
        private readonly IExaminable _examinable;
        public int DeckSize => _examinable?.Deck?.Count ?? 0;
        public int Count { get; }

        public bool CanReorder { get; }

        private readonly List<CardInstance> _examinedCards;
        public IReadOnlyList<CardInstance> ExaminedCards => _examinedCards;

        private readonly List<CardInstance> _currentOrder;
        public List<CardInstance> CurrentOrder => _currentOrder;

        public ExamineResolvable(IExaminable examinable, int count, bool canReorder = false)
        {
            _examinable = examinable;
            Count = count;
            CanReorder = canReorder;

            // Immediately examine the cards.
            _examinedCards = examinable.Deck.ExamineTop(count);
            _currentOrder = new List<CardInstance>(_examinedCards);
        }

        public override void Initialize()
        {
            DialogEvents.RaiseExamineEvent(this);
        }

        /// <summary>
        /// This resolvable can only be resolved via the Examine UI.
        /// </summary>
        public override bool CanCommit(List<IStagedAction> actions) => false;

        public override void Resolve()
        {
            if (CanReorder)
            {
                _examinable.Deck.ReorderExamined(_currentOrder);
            }
        }

        public override StagedActionsState GetUIState(List<IStagedAction> actions)
        {
            // The Examine GUI handles its own button.
            return new StagedActionsState(false, false, false, false);
        }
    }
}
