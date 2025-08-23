using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Items
{
    public class SpyglassTests
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
        public void Spyglass_Usable_With_Perception()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Dire Badger");
            var spyglass = TestUtils.GetCard(_gameServices, "Spyglass");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(spyglass);
            
            var actions = spyglass.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);

            _gameServices.ASM.StageAction(actions[0]);
            var dicePool = _gameServices.ASM.GetStagedDicePool();
            Assert.AreEqual("1d6 + 1d4", dicePool.ToString());
        }

        [Test]
        public void Spyglass_Unusable_Without_Perception()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Soldier");
            var spyglass = TestUtils.GetCard(_gameServices, "Spyglass");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(spyglass);
            
            var actions = spyglass.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }
    }
}
