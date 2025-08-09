using UnityEngine;

namespace PACG.Gameplay
{
    public class DamageProcessor : IProcessor
    {
        private DamageResolvable _resolvable;
        private ContextManager _contexts;
        private GameFlowManager _gameFlow;
        public GameFlowManager GFM => _gameFlow;

        public DamageProcessor(DamageResolvable resolvable, GameServices gameServices)
        {
            _resolvable = resolvable;

            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
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

        public void Finish()
        {
            _contexts.EndResolution();
            GFM.CompleteCurrentPhase();
        }
    }
}
