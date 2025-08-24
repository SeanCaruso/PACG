using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class TraitAccumulator
    {
        private readonly Dictionary<ICard, List<string>> _traits = new();
        public IReadOnlyList<string> Traits => _traits.Values.SelectMany(x => x).ToList();

        private readonly Dictionary<ICard, List<string>> _requiredTraits = new();
        public IReadOnlyList<string> RequiredTraits => _requiredTraits.Values.SelectMany(x => x).ToList();

        private readonly Dictionary<ICard, HashSet<string>> _prohibitedTraits = new();

        public IReadOnlyList<string> ProhibitedTraits(PlayerCharacter pc) =>
            _prohibitedTraits
                .Where(kvp => (kvp.Key as CardInstance)?.Owner == pc)
                .SelectMany(kvp => kvp.Value)
                .ToList();

        public TraitAccumulator(CheckResolvable resolvable)
        {
            if (resolvable == null) return;

            AddTraits(resolvable.Card, resolvable.Card.Traits.ToArray());
            AddTraits(resolvable.Character, resolvable.Character.Traits.ToArray());
        }

        public void AddTraits(ICard card, params string[] traits) => _traits[card] = new List<string>(traits);

        public void AddRequiredTraits(ICard card, params string[] traits) =>
            _requiredTraits[card] = new List<string>(traits);

        public void AddProhibitedTraits(ICard card, params string[] traits)
        {
            if (!_prohibitedTraits.ContainsKey(card))
                _prohibitedTraits.Add(card, new HashSet<string>());
            
            foreach (var trait in traits)
                _prohibitedTraits[card].Add(trait);
        }
    }
}
