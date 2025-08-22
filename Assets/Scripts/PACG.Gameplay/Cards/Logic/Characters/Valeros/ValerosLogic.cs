using PACG.SharedAPI;
using System.Linq;

namespace PACG.Gameplay
{
    public class ValerosLogic : CharacterLogicBase
    {
        private readonly GameServices _gameServices;

        public ValerosLogic(GameServices gameServices) : base(gameServices)
        {
            _gameServices = gameServices;
        }

        public override IResolvable GetEndOfTurnResolvable(PlayerCharacter pc)
        {
            var validCards = pc.Hand.Concat(pc.Discards)
                .Where(card => card.Data.cardType is PF.CardType.Armor or PF.CardType.Weapon)
                .ToList();

            if (validCards.Count <= 0) return null;
            
            GameEvents.SetStatusText("End of Turn Actions.");
            return new PlayerPowerAvailableResolvable(
                pc.CharacterData.Powers[1],
                new ValerosEndOfTurnResolvable(validCards, _gameServices),
                _gameServices);
        }
    }
}
