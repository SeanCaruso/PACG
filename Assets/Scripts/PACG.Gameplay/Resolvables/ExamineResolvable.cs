using PACG.SharedAPI;
using System.Collections.Generic;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ExamineResolvable : BaseResolvable
    {
        private readonly IExaminable _examinable;
        private readonly int _count;
        public int Count => _count;
        private readonly int _deckSize;
        public int DeckSize => _deckSize;
        private readonly bool _canReorder;
        public bool CanReorder => _canReorder;

        private readonly List<CardInstance> _examinedCards;
        public IReadOnlyList<CardInstance> ExaminedCards => _examinedCards;

        private readonly List<CardInstance> _currentOrder;
        public List<CardInstance> CurrentOrder => _currentOrder;

        public ExamineResolvable(IExaminable examinable, int count, int deckSize, bool canReorder = false)
        {
            _examinable = examinable;
            _count = count;
            _deckSize = deckSize;
            _canReorder = canReorder;

            // Immediately examine the cards.
            _examinedCards = examinable.Deck.ExamineTop(count);
            _currentOrder = new(_examinedCards);
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
            if (_canReorder)
            {
                _examinable.Deck.ReorderExamined(_currentOrder);
            }
        }
    }
}
