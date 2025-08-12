using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ValerosLogic : CharacterLogicBase
    {
        private readonly GameServices _gameServices;

        protected ValerosLogic(GameServices gameServices) : base(gameServices)
        {
            _gameServices = gameServices;
        }

        public override List<IResolvable> GetEndOfTurnResolvables(PlayerCharacter pc)
        {
            var validCards = pc.Hand.Concat(pc.Discards)
                .Where(card => card.Data.cardType == PF.CardType.Armor || card.Data.cardType == PF.CardType.Weapon)
                .ToList();

            if (validCards.Count > 0)
                return new() { new ValerosEndOfTurnResolvable(validCards, _gameServices) };
            else
                return new();
        }
    }
}
