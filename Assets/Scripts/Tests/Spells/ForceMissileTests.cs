using System.Linq;
using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Spells
{
    public class ForceMissileTests : BaseTest
    {
        private PlayerCharacter _ezren;
        private CardInstance _forceMissile;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _ezren = TestUtils.GetCharacter(GameServices, "Ezren");
            _forceMissile = TestUtils.GetCard(GameServices, "Force Missile");
            _ezren.AddToHand(_forceMissile);
        }

        [Test]
        public void Force_Missile_On_Own_Check()
        {
            TestUtils.SetupEncounter(GameServices, _ezren, Zombie);
            
            var actions = _forceMissile.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            GameServices.ASM.StageAction(actions[0]);

            var dice = GameServices.Contexts.CheckContext.DicePool(GameServices.ASM.StagedActions);
            Assert.AreEqual("1d12 + 2d4 + 2", dice.ToString());

            var traits = GameServices.Contexts.CheckContext.Traits;
            Assert.IsTrue(traits.Contains("Magic"));
            Assert.IsTrue(traits.Contains("Arcane"));
            Assert.IsTrue(traits.Contains("Attack"));
            Assert.IsTrue(traits.Contains("Force"));
        }

        [Test]
        public void Force_Missile_On_Other_Check()
        {
            TestUtils.SetupEncounter(GameServices, Valeros, Zombie);
            
            var actions = _forceMissile.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            GameServices.ASM.StageAction(actions[0]);

            var dice = GameServices.Contexts.CheckContext.DicePool(GameServices.ASM.StagedActions);
            Assert.AreEqual("1d10 + 2d4 + 2", dice.ToString());

            var traits = GameServices.Contexts.CheckContext.Traits;
            Assert.IsTrue(traits.Contains("Magic"));
            Assert.IsFalse(traits.Contains("Arcane"));
            Assert.IsTrue(traits.Contains("Attack"));
            Assert.IsTrue(traits.Contains("Force"));
        }
    }
}
