using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class ResponseRegistry
    {
        private readonly Dictionary<System.Type, List<CardInstance>> _responseRegistry = new();

        public void RegisterResponses(CardInstance card)
        {
            var logicType = card.Logic.GetType();

            if (typeof(IOnBeforeDiscardResponse).IsAssignableFrom(logicType))
            {
                if (!_responseRegistry.ContainsKey(typeof(IOnBeforeDiscardResponse)))
                    _responseRegistry[typeof(IOnBeforeDiscardResponse)] = new List<CardInstance>();
                _responseRegistry[typeof(IOnBeforeDiscardResponse)].Add(card);
            }
            // ... add checks for other response types ...
        }
        
        public void UnregisterResponses(CardInstance card)
        {
            var logicType = card.Logic.GetType();
            
            if (typeof(IOnBeforeDiscardResponse).IsAssignableFrom(logicType))
            {
                if (_responseRegistry.TryGetValue(typeof(IOnBeforeDiscardResponse), out var cards))
                    cards.Remove(card);
            }
            // ... add checks for other response types ...
        }

        public void TriggerBeforeDiscard(DiscardEventArgs args)
        {
            if (!_responseRegistry.TryGetValue(typeof(IOnBeforeDiscardResponse), out var responseList)) return;
            
            foreach (var card in responseList)
            {
                ((IOnBeforeDiscardResponse)card.Logic).OnBeforeDiscard(card, args);
            }
        }
    }
}
