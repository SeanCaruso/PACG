using PACG.SharedAPI;
using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class ExamineResolvable : BaseResolvable
    {
        private readonly IExaminable _examinable;
        private int DeckSize => _examinable?.Deck?.Count ?? 0;
        private int Count { get; }

        private bool CanReorder { get; }

        private readonly List<CardInstance> _examinedCards;
        private IReadOnlyList<CardInstance> ExaminedCards => _examinedCards;

        private List<CardInstance> CurrentOrder { get; }

        public ExamineResolvable(IExaminable examinable, int count, bool canReorder = false)
        {
            _examinable = examinable;
            Count = count;
            CanReorder = canReorder;

            // Immediately examine the cards.
            _examinedCards = examinable.Deck.ExamineTop(count);
            CurrentOrder = new List<CardInstance>(_examinedCards);
        }

        public override void Initialize()
        {
            var examineContext = new ExamineContext
            {
                ExamineMode = ExamineContext.Mode.Deck,
                Cards = CurrentOrder,
                UnknownCount = DeckSize - Count,
                CanReorder = CanReorder,
                OnClose = () =>
                {
                    // Handle reordering if needed.
                    if (CanReorder)
                    {
                        _examinable.Deck.ReorderExamined(CurrentOrder);
                    }
                }
            };
            
            DialogEvents.RaiseExamineEvent(examineContext);
        }

        /// <summary>
        /// This resolvable can only be resolved via the Examine UI.
        /// </summary>
        public override bool CanCommit(List<IStagedAction> actions) => false;

        public override void Resolve()
        {
            if (CanReorder)
            {
                _examinable.Deck.ReorderExamined(CurrentOrder);
            }
        }

        public override StagedActionsState GetUIState(List<IStagedAction> actions)
        {
            // The Examine GUI handles its own button.
            return new StagedActionsState(false, false, false, false);
        }
    }
}
