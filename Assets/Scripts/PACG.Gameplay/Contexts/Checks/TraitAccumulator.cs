using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class TraitAccumulator
    {
        private readonly Dictionary<ICard, List<string>> _traits = new();
        public IReadOnlyList<string> Traits => _traits.Values.SelectMany(x => x).ToList();

        public TraitAccumulator(CheckResolvable resolvable)
        {
            if (resolvable == null) return;
            
            Add(resolvable.Card, resolvable.Card.Traits.ToArray());
            Add(resolvable.Character, resolvable.Character.Traits.ToArray());
        }
        
        public void Add(ICard card, params string[] traits) => _traits[card] = new List<string>(traits);
        public void Remove(ICard card) => _traits.Remove(card);
    }
}
