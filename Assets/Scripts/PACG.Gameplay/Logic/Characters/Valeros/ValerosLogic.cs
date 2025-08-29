using System.Linq;
using PACG.Data;

namespace PACG.Gameplay
{
    public class ValerosLogic : CharacterLogicBase
    {
        // Dependency injections
        private readonly GameServices _gameServices;

        public ValerosLogic(GameServices gameServices)
        {
            _gameServices = gameServices;
        }

        public override CharacterPower? GetEndOfTurnPower(PlayerCharacter pc)
        {
            var validCards = pc.Hand.Concat(pc.Discards)
                .Where(card => card.Data.cardType is CardType.Armor or CardType.Weapon)
                .ToList();

            if (validCards.Count <= 0)
            {
                return null;
            }

            var rechargePower = pc.CharacterData.Powers[1];


            rechargePower.OnActivate = () =>
            {
                var resolvable = new ValerosEndOfTurnResolvable(validCards, rechargePower, _gameServices);
                _gameServices.GameFlow.Interrupt(new NewResolvableProcessor(resolvable, _gameServices));
                _gameServices.ASM.Commit();
            };

            return rechargePower;
        }
    }
}
