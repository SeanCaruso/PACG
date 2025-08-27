using System.Collections.Generic;
using PACG.SharedAPI;
using System.Linq;
using PACG.Data;

namespace PACG.Gameplay
{
    public class ValerosLogic : CharacterLogicBase
    {
        // Dependency injections
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;
        
        public ValerosLogic(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameServices = gameServices;
        }

        public override CharacterPower? GetEndOfTurnPower(PlayerCharacter pc)
        {
            var validCards = pc.Hand.Concat(pc.Discards)
                .Where(card => card.Data.cardType is PF.CardType.Armor or PF.CardType.Weapon)
                .ToList();

            var rechargePower = pc.CharacterData.Powers[1];

            if (validCards.Count <= 0
                && _contexts.TurnContext?.PerformedCharacterPowers.Contains(rechargePower) == false)
            {
                return null;
            }

            rechargePower.OnActivate = () =>
            {
                var resolvable = new ValerosEndOfTurnResolvable(validCards, _gameServices);
                _gameServices.GameFlow.Interrupt(new NewResolvableProcessor(resolvable, _gameServices));
                _gameServices.ASM.Commit();
            };
            
            return rechargePower;
        }
    }
}
