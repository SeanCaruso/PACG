using System.Linq;
using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Allies
{
    public class LookoutTests
    {
        private GameServices _gameServices;
        private CardInstance _lookout;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _gameServices = TestUtils.CreateGameServices();
        }
        
        [SetUp]
        public void Setup()
        {
            _lookout = TestUtils.GetCard(_gameServices, "Lookout");
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
        public void Lookout_Combat_NoActions()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Zombie");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(_lookout);
            
            var actions = _lookout.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Lookout_Combat_OneAction()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Dire Badger");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(_lookout);
            
            var actions = _lookout.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            _gameServices.ASM.StageAction(actions[0]);

            var dicePool = _gameServices.Contexts.CheckContext.DicePool(_gameServices.ASM.StagedActions);
            Assert.AreEqual("2d4", dicePool.ToString());
        }
    }
}
