using System.Collections.Generic;
using PACG.Core;
using UnityEngine;

namespace PACG.Gameplay
{
    // TODO: Implement giving cards when we have multiple PCs.
    public class GiveCardAction : IStagedAction
    {
        public CardInstance Card { get; }
        public ActionType ActionType => ActionType.Discard;
        public bool IsFreely => false;
        public Dictionary<string, object> ActionData { get; } = new();

        private readonly PlayerCharacter _targetPc;

        public GiveCardAction(CardInstance card, PlayerCharacter targetPc)
        {
            Card = card;
            _targetPc = targetPc;
        }

        public void Commit()
        {
            var targetName = _targetPc?.CharacterData.CharacterName ?? "nonexistent PC";
            Debug.Log($"{Card.Data.cardName} given to {targetName}.");
        }
    }
}
