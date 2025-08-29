using NUnit.Framework;
using PACG.Core;
using PACG.Gameplay;

namespace Tests.Items
{
    public class BracersOfProtectionTests : BaseTest
    {
        private CardInstance _bracers;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _bracers = TestUtils.GetCard(GameServices, "Bracers of Protection");
            Ezren.AddToHand(_bracers);
        }

        [Test]
        public void Bracers_Of_Protection_Two_Actions_Combat_Damage()
        {
            var resolvable = new DamageResolvable(Ezren, 2);
            GameServices.Contexts.NewResolvable(resolvable);
            
            var actions = _bracers.GetAvailableActions();
            Assert.AreEqual(2, actions.Count);
            Assert.AreEqual(ActionType.Reveal, actions[0].ActionType);
            Assert.AreEqual(ActionType.Recharge, actions[1].ActionType);
            
            GameServices.ASM.StageAction(actions[0]);
            Assert.IsFalse(resolvable.CanCommit(GameServices.ASM.StagedActions));
            
            actions = _bracers.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Recharge, actions[0].ActionType);
            
            GameServices.ASM.StageAction(actions[0]);
            Assert.IsTrue(resolvable.CanCommit(GameServices.ASM.StagedActions));
        }

        [Test]
        public void Bracers_Of_Protection_One_Actions_Other_Damage()
        {
            var resolvable = new DamageResolvable(Ezren, 1, "Other");
            GameServices.Contexts.NewResolvable(resolvable);
            
            var actions = _bracers.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Recharge, actions[0].ActionType);
            
            GameServices.ASM.StageAction(actions[0]);
            Assert.IsTrue(resolvable.CanCommit(GameServices.ASM.StagedActions));
        }
    }
}
