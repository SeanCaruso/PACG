using NUnit.Framework;
using PACG.Data;
using PACG.Gameplay;

namespace Tests.Scenarios
{
    public class RumbleRoadTests : BaseTest
    {
        private ScenarioData _data;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _data = TestUtils.GetScenario(GameServices, "Rumble Road").data;
        }

        [Test]
        public void Rumble_Road_Heal_Power_Available()
        {
            GameServices.Contexts.NewGame(new GameContext(1, _data, GameServices));
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, Caravan);
            GameServices.Contexts.NewTurn(new TurnContext(Valeros));
            Valeros.Discard(Longsword);
            Valeros.Location.ShuffleIn(Zombie, true);
            
            var processor = new Turn_StartTurnProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.GameContext.ScenarioLogic.HasAvailableAction);
            Assert.IsTrue(GameServices.Contexts.TurnContext.HasScenarioTurnAction);
            Assert.IsTrue(GameServices.Contexts.TurnContext.CanUseScenarioTurnAction);
        }
    }
}
