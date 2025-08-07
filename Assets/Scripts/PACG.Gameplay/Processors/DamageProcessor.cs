using UnityEngine;

namespace PACG.Gameplay
{
    public class DamageProcessor : IProcessor
    {
        DamageResolvable _resolvable;

        public DamageProcessor(DamageResolvable resolvable)
        {
            _resolvable = resolvable;
        }

        public void Execute()
        {
            // The damage was already "resolved" by the committed actions.
            // DefaultDamageAction.Commit() already discarded the cards.
            // PlayCardActions already applied their damage reduction effects.

            // All we need to do is verify the damage was fully absorbed
            if (_resolvable.Amount > 0)
            {
                Debug.LogWarning($"Unresolved damage: {_resolvable.Amount}");
            }
            else
            {
                Debug.Log("Damage successfully resolved.");
            }

            // Damage handling complete - no more processors needed
        }
    }
}
