using PACG.Data;

namespace PACG.Gameplay
{
    public class EzrenLogic : CharacterLogicBase
    {
        // Dependency injection
        private readonly ActionStagingManager _asm;
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;
        
        public EzrenLogic(GameServices gameServices)
        {
            _asm = gameServices.ASM;
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override CharacterPower? GetStartOfTurnPower(PlayerCharacter pc)
        {
            if (pc.Deck.Count == 0) return null;

            var power = pc.CharacterData.Powers[0];
            var topCard = pc.Deck.ExamineTop(1)[0];
            PlayerChoiceResolvable resolvable;

            if (topCard.CardType == CardType.Spell)
            {
                resolvable = new PlayerChoiceResolvable($"Draw {topCard.Name}?",
                    new PlayerChoiceResolvable.ChoiceOption("Draw", () => pc.AddToHand(topCard)),
                    new PlayerChoiceResolvable.ChoiceOption("Reload", () => pc.Reload(topCard)));
            }
            else
            {
                resolvable = new PlayerChoiceResolvable("",
                    new PlayerChoiceResolvable.ChoiceOption("OK", () => pc.Reload(topCard)));
            }
            resolvable.Card = topCard;

            var processor = new NewResolvableProcessor(resolvable, _gameServices);

            power.OnActivate = () =>
            {
                _contexts.TurnContext.PerformedCharacterPowers.Add(power);
                _gameFlow.Interrupt(processor);
                _asm.Commit();
            };

            return power;
        }
    }
}
