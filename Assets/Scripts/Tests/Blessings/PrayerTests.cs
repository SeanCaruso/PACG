using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Blessings
{
    public class PrayerTests
    {
        
        private GameServices _gameServices;
        
        [SetUp]
        public void Setup()
        {
            _gameServices = TestUtils.CreateGameServices();
        }

        [TearDown]
        public void TearDown()
        {
            _gameServices.Contexts.EndCheck();
            _gameServices.Contexts.EndResolvable();
            _gameServices.Contexts.EndEncounter();
            _gameServices.Contexts.EndTurn();
        }

        [Test]
        public void Prayer_CanBless_Combat()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Zombie");
            var prayer = TestUtils.GetCard(_gameServices, "Prayer");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(prayer);
            
            var actions = prayer.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
        }

        [Test]
        public void Prayer_CanBless_Skill()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Soldier");
            var prayer = TestUtils.GetCard(_gameServices, "Prayer");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(prayer);
            
            var actions = prayer.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
        }

        [Test]
        public void Prayer_NotFreely()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Soldier");
            var prayer = TestUtils.GetCard(_gameServices, "Prayer");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(prayer);
            
            var actions = prayer.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            _gameServices.ASM.StageAction(actions[0]);
            
            var prayer2 = TestUtils.GetCard(_gameServices, "Prayer");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(prayer2);
            actions = prayer2.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Prayer_Bless_Valeros_Combat()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Zombie");
            var prayer = TestUtils.GetCard(_gameServices, "Prayer");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(prayer);
            
            var actions = prayer.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            _gameServices.ASM.StageAction(actions[0]);
            var dicePool = _gameServices.ASM.GetStagedDicePool();
            Assert.AreEqual("2d10 + 2", dicePool.ToString());
        }
    }
}
