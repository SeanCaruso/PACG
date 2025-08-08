using UnityEngine;

namespace PACG.Gameplay
{
    // TODO: Implement giving cards when we have multiple PCs.
    public class GiveCardAction : IStagedAction
    {
        public CardInstance Card { get; }
        public PF.ActionType ActionType => PF.ActionType.Discard;
        public bool IsFreely => false;

        private readonly PlayerCharacter _targetPc;

        public GiveCardAction(CardInstance card, PlayerCharacter targetPc)
        {
            Card = card;
            _targetPc = targetPc;
        }

        public void Commit(CheckContext _ = null)
        {
            string targetName = _targetPc?.characterData.characterName ?? "nonexistent PC";
            Debug.Log($"{Card.Data.cardName} given to {targetName}.");
        }

        public void OnStage() { }

        public void OnUndo() { }
    }
}
