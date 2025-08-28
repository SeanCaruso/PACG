using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Allies
{
    public class LookoutTests : BaseTest
    {
        private CardInstance _lookout;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _lookout = TestUtils.GetCard(GameServices, "Lookout");
        }

        [Test]
        public void Lookout_Combat_NoActions()
        {
            TestUtils.SetupEncounter(GameServices, "Valeros", "Zombie");
            GameServices.Contexts.EncounterContext.Character.AddToHand(_lookout);
            
            var actions = _lookout.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Lookout_Combat_OneAction()
        {
            TestUtils.SetupEncounter(GameServices, "Valeros", "Dire Badger");
            GameServices.Contexts.EncounterContext.Character.AddToHand(_lookout);
            
            var actions = _lookout.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            GameServices.ASM.StageAction(actions[0]);

            var dicePool = GameServices.Contexts.CheckContext.DicePool(GameServices.ASM.StagedActions);
            Assert.AreEqual("2d4", dicePool.ToString());
        }

        [Test]
        public void Lookout_Not_Usable_During_Damage()
        {
            Valeros.AddToHand(_lookout);
            
            var damage = new DamageResolvable(Valeros, 1, "Magic");
            GameServices.Contexts.NewResolvable(damage);
            
            var actions = _lookout.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }
    }
}
