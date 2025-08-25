using System.Linq;
using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Scourges
{
    public class WoundedTests : BaseTest
    {
        private PlayerCharacter _valeros;
        private CardInstance _longsword;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _valeros = TestUtils.GetCharacter(GameServices, "Valeros");
            _longsword = TestUtils.GetCard(GameServices, "Longsword");
            _longsword.Owner = _valeros;
            _valeros.Reload(_longsword);
            
            var gameContext = new GameContext(1, GameServices.Cards);
            GameServices.Contexts.NewGame(gameContext);
            GameServices.Contexts.NewTurn(new TurnContext(_valeros));
        }

        [Test]
        public void Wounded_Scourged_Discards_At_Start_Of_Turn()
        {
            _valeros.AddScourge(ScourgeType.Wounded);
            GameServices.GameFlow.StartPhase(new StartTurnController(_valeros, GameServices), "Turn");
            
            Assert.AreEqual(0, _valeros.Deck.Count);
            Assert.AreEqual(1, _valeros.Discards.Count);
            Assert.AreEqual(_longsword, _valeros.Discards[0]);
        }

        [Test]
        public void Wounded_Removal_Prompt_On_Heal()
        {
            _valeros.AddScourge(ScourgeType.Wounded);
            _valeros.Heal(1);
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            resolvable.Options[1].Action.Invoke();
        }

        [Test]
        public void Wounded_Removed()
        {
            _valeros.AddScourge(ScourgeType.Wounded);
            ScourgeRules.PromptForWoundedRemoval(_valeros, GameServices);
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            resolvable.Options[1].Action.Invoke();
            Assert.IsTrue(_valeros.ActiveScourges.Count == 1);
            Assert.IsTrue(_valeros.ActiveScourges.Contains(ScourgeType.Wounded));
            
            resolvable.Options[0].Action.Invoke();
            Assert.IsTrue(_valeros.ActiveScourges.Count == 0);
        }
    }
}
