using PACG.SharedAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ValerosLogic : CharacterLogicBase
    {
        private readonly GameServices _gameServices;

        public ValerosLogic(GameServices gameServices) : base(gameServices)
        {
            _gameServices = gameServices;
        }

        public override List<IResolvable> GetEndOfTurnResolvables(PlayerCharacter pc)
        {
            var validCards = pc.Hand.Concat(pc.Discards)
                .Where(card => card.Data.cardType == PF.CardType.Armor || card.Data.cardType == PF.CardType.Weapon)
                .ToList();

            if (validCards.Count > 0)
            {
                GameEvents.SetStatusText("End of Turn Actions.");
                return new() { new PlayerPowerAvailableResolvable(pc.CharacterData.powers[1], new ValerosEndOfTurnResolvable(validCards, _gameServices), _gameServices) };
            }
            else
                return new();
        }
    }
}
