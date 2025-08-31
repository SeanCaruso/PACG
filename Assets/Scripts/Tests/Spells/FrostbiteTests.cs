using System.Linq;
using NUnit.Framework;
using PACG.Core;
using PACG.Gameplay;

namespace Tests.Spells
{
    public class FrostbiteTests : BaseTest
    {
        private PlayerCharacter _ezren;
        private CardInstance _frostbite;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _ezren = TestUtils.GetCharacter(GameServices, "Ezren");
            _frostbite = TestUtils.GetCard(GameServices, "Frostbite");
            _ezren.AddToHand(_frostbite);
        }

        [Test]
        public void Frostbite_On_Own_Check()
        {
            TestUtils.SetupEncounter(GameServices, _ezren, Zombie);
            
            var actions = _frostbite.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            GameServices.ASM.StageAction(actions[0]);

            var dice = GameServices.Contexts.CheckContext.DicePool(GameServices.ASM.StagedActions);
            Assert.AreEqual("1d12 + 2d4 + 2", dice.ToString());

            var traits = GameServices.Contexts.CheckContext.Traits;
            Assert.IsTrue(traits.Contains("Magic"));
            Assert.IsTrue(traits.Contains("Arcane"));
            Assert.IsTrue(traits.Contains("Divine"));
            Assert.IsTrue(traits.Contains("Attack"));
            Assert.IsTrue(traits.Contains("Cold"));
        }

        [Test]
        public void Frostbite_Unusable_On_Other_Check()
        {
            TestUtils.SetupEncounter(GameServices, Valeros, Zombie);
            
            var actions = _frostbite.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Frostbite_Reduced_Damage_By_One()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, Zombie));

            var frostbiteAction = new PlayCardAction(_frostbite, ActionType.Banish, null);
            frostbiteAction.Commit();
            
            Assert.AreEqual(1, GameServices.Contexts.EncounterContext.ResolvableModifiers.Count);
            
            var resolvable = new DamageResolvable(_ezren, 2, GameServices, "Magic");
            GameServices.Contexts.NewResolvable(resolvable);
            
            Assert.AreEqual(1, (GameServices.Contexts.CurrentResolvable as DamageResolvable)?.Amount);
        }
    }
}
