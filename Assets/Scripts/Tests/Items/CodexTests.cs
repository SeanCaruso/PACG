using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Items
{
    public class CodexTests : BaseTest
    {
        private CardInstance _codex;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _codex = TestUtils.GetCard(GameServices, "Codex");
            Ezren.AddToHand(_codex);
        }

        [Test]
        public void Sages_Journal_No_Actions_Vs_Bane()
        {
            TestUtils.SetupEncounter(GameServices, Ezren, Zombie);
            
            var actions = _codex.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Sages_Journal_Two_Actions_Vs_Own_Boon()
        {
            TestUtils.SetupEncounter(GameServices, Ezren, Longsword);
            
            var actions = _codex.GetAvailableActions();
            Assert.AreEqual(2, actions.Count);

            var revealMod = _codex.Logic.GetCheckModifier(actions[0]);
            Assert.AreEqual(0, revealMod.AddedDice.Count);
            Assert.AreEqual(1, revealMod.AddedBonus);
            
            var buryMod = _codex.Logic.GetCheckModifier(actions[1]);
            Assert.AreEqual(1, buryMod.AddedDice.Count);
            Assert.AreEqual(12, buryMod.AddedDice[0]);
            Assert.AreEqual(2, buryMod.AddedBonus);
        }

        [Test]
        public void Sages_Journal_One_Actions_Vs_Local_Boon()
        {
            TestUtils.SetupEncounter(GameServices, Valeros, Longsword);
            
            GameServices.Contexts.GameContext.SetPcLocation(Ezren, Valeros.Location);
            
            var actions = _codex.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            var buryMod = _codex.Logic.GetCheckModifier(actions[0]);
            Assert.AreEqual(1, buryMod.AddedDice.Count);
            Assert.AreEqual(12, buryMod.AddedDice[0]);
            Assert.AreEqual(2, buryMod.AddedBonus);
        }

        [Test]
        public void Sages_Journal_No_Actions_Vs_Distant_Boon()
        {
            TestUtils.SetupEncounter(GameServices, Valeros, Longsword);
            
            var campsite = TestUtils.GetLocation(GameServices, "Campsite");
            GameServices.Contexts.GameContext.SetPcLocation(Ezren, campsite);
            Assert.AreNotEqual(Ezren.Location, Valeros.Location);
            
            var actions = _codex.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }
    }
}
