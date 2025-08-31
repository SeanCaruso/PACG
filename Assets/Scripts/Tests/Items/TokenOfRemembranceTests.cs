using NUnit.Framework;
using PACG.Core;
using PACG.Gameplay;

namespace Tests.Items
{
    public class TokenOfRemembranceTests : BaseTest
    {
        private CardInstance _frostbite;
        private CardInstance _token;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _token = TestUtils.GetCard(GameServices, "Token of Remembrance");
            Ezren.AddToHand(_token);

            _frostbite = TestUtils.GetCard(GameServices, "Frostbite");
            _frostbite.Owner = Ezren;
        }

        [Test]
        public void Token_Of_Remembrance_Recharge_For_Spell()
        {
            _frostbite.CurrentLocation = CardLocation.Recovery;

            var processor = new Turn_RecoveryProcessor(GameServices);
            GameServices.Contexts.NewTurn(new TurnContext(Ezren));
            processor.Execute();

            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            var resolvable = (PlayerChoiceResolvable)GameServices.Contexts.CurrentResolvable;
            GameServices.Contexts.EndResolvable();
            resolvable.Options[0].Action.Invoke();
            GameServices.GameFlow.Process();

            var actions = _token.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Recharge, actions[0].ActionType);

            var modifier = (actions[0] as PlayCardAction)?.CheckModifier;
            Assert.IsNotNull(modifier);
            Assert.AreEqual(1, modifier.AddedDice.Count);
            Assert.AreEqual(8, modifier.AddedDice[0]);
        }

        [Test]
        public void Token_Of_Remembrance_No_Recharge_For_Non_Spell()
        {
            var clockworkServant = TestUtils.GetCard(GameServices, "Clockwork Servant");
            clockworkServant.Owner = Ezren;
            clockworkServant.CurrentLocation = CardLocation.Recovery;

            var processor = new Turn_RecoveryProcessor(GameServices);
            GameServices.Contexts.NewTurn(new TurnContext(Ezren));
            processor.Execute();

            var actions = _token.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Token_Of_Remembrance_No_Actions_With_No_Discarded_Spell()
        {
            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            Ezren.Discard(soldier);

            var actions = _token.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Token_Of_Remembrance_Reloads_Discarded_Spell()
        {
            Ezren.Discard(_frostbite);

            var actions = _token.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Bury, actions[0].ActionType);

            _token.Logic.OnCommit(actions[0]);
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is TokenOfRemembranceResolvable);

            var reloadActions =
                GameServices.Contexts.CurrentResolvable.GetAdditionalActionsForCard(_frostbite);
            Assert.AreEqual(1, reloadActions.Count);
            Assert.AreEqual(ActionType.Reload, reloadActions[0].ActionType);
        }
    }
}
