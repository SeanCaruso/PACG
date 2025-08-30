using System.Collections.Generic;
using NUnit.Framework;
using PACG.Core;
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
            GameServices.Contexts.NewGame(new GameContext(1, _data, GameServices));
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, Caravan);
            GameServices.Contexts.NewTurn(new TurnContext(Valeros));
        }

        [Test]
        public void Rumble_Road_Heal_Power_Available()
        {
            Valeros.Discard(Longsword);
            Valeros.Location.ShuffleIn(Zombie, true);
            
            var processor = new Turn_StartTurnProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.GameContext.ScenarioLogic.HasAvailableAction);
            Assert.IsTrue(GameServices.Contexts.TurnContext.HasScenarioTurnAction);
            Assert.IsTrue(GameServices.Contexts.TurnContext.CanUseScenarioTurnAction);
        }

        [Test]
        public void Rumble_Road_Dire_Wolf_Allows_Close()
        {
            // Setup puts Valeros at the Caravan.
            var direWolf = TestUtils.GetCard(GameServices, "Dire Wolf");
            GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, direWolf));

            GameServices.Contexts.EncounterContext.CheckResult = new CheckResult(
                1, 0, Valeros, true, Skill.Melee, new List<string>());
            var processor = new Encounter_EndEncounterProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            Assert.AreEqual("Close location?", resolvable.Prompt);
            Assert.AreEqual(2, resolvable.Options.Count);
            Assert.AreEqual("Close", resolvable.Options[0].Label);
            Assert.AreEqual("Skip", resolvable.Options[1].Label);
        }
    }
}
