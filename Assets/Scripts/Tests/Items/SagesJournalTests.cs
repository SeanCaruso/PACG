using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Items
{
    public class SagesJournalTests : BaseTest
    {
        private CardInstance _sagesJournal;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _sagesJournal = TestUtils.GetCard(GameServices, "Sage's Journal");
            Ezren.AddToHand(_sagesJournal);
        }

        [Test]
        public void Sages_Journal_No_Actions_Vs_Ally()
        {
            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            TestUtils.SetupEncounter(GameServices, Ezren, soldier);
            
            var actions = _sagesJournal.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Sages_Journal_Two_Actions_Vs_Own_Story_Bane()
        {
            var direWolf = TestUtils.GetCard(GameServices, "Dire Wolf");
            TestUtils.SetupEncounter(GameServices, Ezren, direWolf);
            
            var actions = _sagesJournal.GetAvailableActions();
            Assert.AreEqual(2, actions.Count);

            var revealMod = _sagesJournal.Logic.GetCheckModifier(actions[0]);
            Assert.AreEqual(1, revealMod.AddedDice.Count);
            Assert.AreEqual(4, revealMod.AddedDice[0]);
            
            var buryMod = _sagesJournal.Logic.GetCheckModifier(actions[1]);
            Assert.AreEqual(1, buryMod.AddedDice.Count);
            Assert.AreEqual(12, buryMod.AddedDice[0]);
            Assert.AreEqual(2, buryMod.AddedBonus);
        }

        [Test]
        public void Sages_Journal_One_Actions_Vs_Local_Story_Bane()
        {
            var direWolf = TestUtils.GetCard(GameServices, "Dire Wolf");
            TestUtils.SetupEncounter(GameServices, Valeros, direWolf);
            
            GameServices.Contexts.GameContext.SetPcLocation(Ezren, Valeros.Location);
            
            var actions = _sagesJournal.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            var buryMod = _sagesJournal.Logic.GetCheckModifier(actions[0]);
            Assert.AreEqual(1, buryMod.AddedDice.Count);
            Assert.AreEqual(12, buryMod.AddedDice[0]);
            Assert.AreEqual(2, buryMod.AddedBonus);
        }

        [Test]
        public void Sages_Journal_No_Actions_Vs_Distant_Story_Bane()
        {
            var direWolf = TestUtils.GetCard(GameServices, "Dire Wolf");
            TestUtils.SetupEncounter(GameServices, Valeros, direWolf);
            
            var campsite = TestUtils.GetLocation(GameServices, "Campsite");
            GameServices.Contexts.GameContext.SetPcLocation(Ezren, campsite);
            Assert.AreNotEqual(Ezren.Location, Valeros.Location);
            
            var actions = _sagesJournal.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }
    }
}
