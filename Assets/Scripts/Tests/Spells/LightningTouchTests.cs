using System.Linq;
using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Spells
{
    public class LightningTouchTests : BaseTest
    {
        private PlayerCharacter _ezren;
        private CardInstance _lightningTouch;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _ezren = TestUtils.GetCharacter(GameServices, "Ezren");
            _lightningTouch = TestUtils.GetCard(GameServices, "Lightning Touch");
            _ezren.AddToHand(_lightningTouch);
        }

        [Test]
        public void Lightning_Touch_On_Own_Check()
        {
            TestUtils.SetupEncounter(GameServices, _ezren, Zombie);
            
            var actions = _lightningTouch.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            GameServices.ASM.StageAction(actions[0]);

            var dice = GameServices.Contexts.CheckContext.DicePool(GameServices.ASM.StagedActions);
            Assert.AreEqual("1d12 + 2d4 + 2", dice.ToString());

            var traits = GameServices.Contexts.CheckContext.Traits;
            Assert.IsTrue(traits.Contains("Magic"));
            Assert.IsTrue(traits.Contains("Arcane"));
            Assert.IsTrue(traits.Contains("Attack"));
            Assert.IsTrue(traits.Contains("Electricity"));
        }

        [Test]
        public void Lightning_Touch_Unusable_On_Other_Check()
        {
            TestUtils.SetupEncounter(GameServices, Valeros, Zombie);
            
            var actions = _lightningTouch.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Lightning_Touch_Disables_Monster_After_Acting()
        {
            // TODO: Implement this when we have a monster with after-acting powers.
        }
    }
}
