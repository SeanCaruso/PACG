using System.Collections.Generic;
using NUnit.Framework;
using PACG.Gameplay;
using UnityEngine;

namespace Tests.Weapons
{
    public class QuarterstaffTests
    {
        private GameServices _gameServices;
    
        private PlayerCharacter _valeros;
        private CardInstance _quarterstaff;

        [SetUp]
        public void Setup()
        {
            CardUtils.Initialize(1);
        
            _gameServices = TestUtils.CreateGameServices();
            _valeros = TestUtils.GetCharacter(_gameServices, "Valeros");
            _quarterstaff = TestUtils.GetCard(_gameServices, "Quarterstaff");
            _valeros.AddToHand(_quarterstaff);
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
        public void Quarterstaff_Combat_ProficientActions()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Zombie");
            _gameServices.Contexts.EncounterContext.Character.AddToHand(_quarterstaff);

            // Before staging, any PC has two actions.
            var actions = _quarterstaff.GetAvailableActions();
            Assert.AreEqual(2, actions.Count);
            Assert.AreEqual(PF.ActionType.Reveal, actions[0].ActionType);
            Assert.AreEqual(PF.ActionType.Discard, actions[1].ActionType);

            // After staging, any PC has one action.
            _gameServices.ASM.StageAction(actions[0]);
            actions = _quarterstaff.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(PF.ActionType.Discard, actions[0].ActionType);
        }

        [Test]
        public void Quarterstaff_EvadeObstacle()
        {
            // Set up a new encounter with an Obstacle barrier.
            _gameServices.Contexts.NewTurn(new TurnContext(_valeros));

            var encounterData = ScriptableObject.CreateInstance<BaneCardData>();
            encounterData.cardType = PF.CardType.Barrier;
            encounterData.traits = new List<string> { "Obstacle", "Other Trait" };

            var encounterInstance = new CardInstance(encounterData, null);

            _gameServices.Contexts.NewEncounter(new EncounterContext(_valeros, encounterInstance));

            // Start the encounter.
            _gameServices.GameFlow.StartPhase(new EncounterController(_valeros, encounterInstance, _gameServices),
                "Encounter");

            // Check that the game pauses when reaching a Generic Resolvable.
            Assert.IsNotNull(_gameServices.Contexts.CurrentResolvable);
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is GenericResolvable);

            // Check that the quarterstaff has one evade action.
            var actions = _quarterstaff.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(PF.ActionType.Discard, actions[0].ActionType);

            // Stage and commit the evade action.
            _gameServices.ASM.StageAction(actions[0]);
            _gameServices.ASM.Commit();

            // Check that the encounter ends.
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable == null);
            Assert.IsTrue(_gameServices.Contexts.EncounterContext == null);
        }

        [Test]
        public void Quarterstaff_EvadeTrap()
        {
            // Set up a new encounter with an Obstacle barrier.
            var valeros = TestUtils.GetCharacter(_gameServices, "Valeros");
            _gameServices.Contexts.NewTurn(new TurnContext(_valeros));

            var encounterData = ScriptableObject.CreateInstance<BaneCardData>();
            encounterData.cardType = PF.CardType.Barrier;
            encounterData.traits = new List<string> { "Trap", "Other Trait" };

            var encounterInstance = new CardInstance(encounterData, null);

            _gameServices.Contexts.NewEncounter(new EncounterContext(_valeros, encounterInstance));

            // Start the encounter.
            _gameServices.GameFlow.StartPhase(new EncounterController(_valeros, encounterInstance, _gameServices),
                "Encounter");

            // Check that the game pauses when reaching a Generic Resolvable.
            Assert.IsNotNull(_gameServices.Contexts.CurrentResolvable);
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is GenericResolvable);

            // Check that the quarterstaff has one evade action.
            var actions = _quarterstaff.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(PF.ActionType.Discard, actions[0].ActionType);

            // Stage and commit the evade action.
            _gameServices.ASM.StageAction(actions[0]);
            _gameServices.ASM.Commit();

            // Check that the encounter ends.
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable == null);
            Assert.IsTrue(_gameServices.Contexts.EncounterContext == null);
        }

        [Test]
        public void Quarterstaff_NoEvadeNonObstacleOrTrap()
        {
            // Set up a new encounter with an Obstacle barrier.
            var valeros = TestUtils.GetCharacter(_gameServices, "Valeros");
            _gameServices.Contexts.NewTurn(new TurnContext(_valeros));

            var encounterData = ScriptableObject.CreateInstance<BaneCardData>();
            encounterData.cardType = PF.CardType.Barrier;
            encounterData.traits = new List<string> { "Not a Trap", "Not an Obstacle" };
            encounterData.checkRequirement = new CheckRequirement
            {
                mode = CheckMode.Single,
                checkSteps = new List<CheckStep>
                    { new() { category = CheckCategory.Skill, allowedSkills = new List<PF.Skill>() } }
            };

            var encounterInstance = new CardInstance(encounterData, new ZombieLogic(_gameServices));

            _gameServices.Contexts.NewEncounter(new EncounterContext(_valeros, encounterInstance));

            // Start the encounter.
            _gameServices.GameFlow.StartPhase(new EncounterController(_valeros, encounterInstance, _gameServices),
                "Encounter");

            // Check that the game pauses when reaching the required check resolvable, and that the quarterstaff
            // doesn't prompt to evade an invalid barrier.
            Assert.IsNotNull(_gameServices.Contexts.CurrentResolvable);
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is CheckResolvable);

            // Check that the quarterstaff has one evade action.
            var actions = _quarterstaff.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }
    }
}
