using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PACG.Core;
using PACG.Data;
using PACG.Gameplay;
using UnityEngine;

namespace Tests.Allies
{
    public class CatTests
    {
        private GameServices _gameServices;
        private CardData _catData;
        private CardInstance _catInstance;

        private CharacterData _valerosData;
        private PlayerCharacter _valeros;

        [SetUp]
        public void Setup()
        {
            _gameServices = TestUtils.CreateGameServices();
            _gameServices.Contexts.NewGame(new GameContext(1, _gameServices.Cards));
        
            _valerosData = TestUtils.LoadCardData<CharacterData>("Valeros");
            _valeros = new PlayerCharacter(_valerosData, _gameServices.Logic.GetLogic<CharacterLogicBase>(_valerosData.CharacterName), _gameServices);
            _gameServices.Contexts.GameContext.SetPcLocation(_valeros, new Location(ScriptableObject.CreateInstance<LocationData>(), null, _gameServices));
        
            _catData = TestUtils.LoadCardData<CardData>("Cat");
            _catInstance = _gameServices.Cards.New(_catData, _valeros);
            _catInstance.CurrentLocation = CardLocation.Hand;
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
        public void Cat_CanRechargeVsSpell()
        {
            // Set up a new encounter with a spell.
            _gameServices.Contexts.NewTurn(new TurnContext(_valeros));

            var encounterData = ScriptableObject.CreateInstance<BoonCardData>();
            encounterData.cardType = CardType.Spell;
            encounterData.traits = new List<string> { "Arcane", "Magic" };
            encounterData.checkRequirement = new CheckRequirement
            {
                mode = CheckMode.Single,
                checkSteps = new List<CheckStep>
                    { new() { category = CheckCategory.Skill, allowedSkills = new List<Skill> { Skill.Arcane } } }
            };

            var encounterInstance = new CardInstance(encounterData, new ZombieLogic(_gameServices));

            _gameServices.Contexts.NewEncounter(new EncounterContext(_valeros, encounterInstance));

            // Start the encounter.
            _gameServices.GameFlow.StartPhase(new EncounterController(_valeros, encounterInstance, _gameServices),
                "Encounter");

            // Check that the game pauses when reaching the required check resolvable.
            Assert.IsNotNull(_gameServices.Contexts.CurrentResolvable);
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is CheckResolvable);

            // Check that the Cat has one recharge action.
            var actions = _catInstance.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Recharge, actions[0].ActionType);

            var modifier = _catInstance.Logic.GetCheckModifier(actions[0]);
            var numD4 = modifier.AddedDice.Count(d => d == 4);
            Assert.AreEqual(1, numD4);
        }

        [Test]
        public void Cat_CanNotRechargeVsNonSpell()
        {
            // Set up a new encounter with an Obstacle barrier.
            _gameServices.Contexts.NewTurn(new TurnContext(_valeros));

            var encounterData = ScriptableObject.CreateInstance<BoonCardData>();
            encounterData.cardType = CardType.Item;
            encounterData.traits = new List<string> { "Magic" };
            encounterData.checkRequirement = new CheckRequirement
            {
                mode = CheckMode.Single,
                checkSteps = new List<CheckStep>
                    { new() { category = CheckCategory.Skill, allowedSkills = new List<Skill>() } }
            };

            var encounterInstance = new CardInstance(encounterData, new ZombieLogic(_gameServices));

            _gameServices.Contexts.NewEncounter(new EncounterContext(_valeros, encounterInstance));

            // Start the encounter.
            _gameServices.GameFlow.StartPhase(new EncounterController(_valeros, encounterInstance, _gameServices),
                "Encounter");

            // Check that the game pauses when reaching the required check resolvable.
            Assert.IsNotNull(_gameServices.Contexts.CurrentResolvable);
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is CheckResolvable);

            // Check that the Cat doesn't have a recharge action.
            var actions = _catInstance.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Cat_ExploreWithoutMagic()
        {
            _gameServices.Contexts.NewTurn(new TurnContext(_valeros));
        
            // Set up the location with a zombie card.
            var zombieData = TestUtils.LoadCardData<CardData>("Zombie");
            var zombieInstance = _gameServices.Cards.New(zombieData);
            _valeros.Location.ShuffleIn(zombieInstance, false);
        
            // Check that we can use the Cat to explore.
            var actions = _catInstance.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Discard, actions[0].ActionType);
        
            // Stage the explore action and explore.
            _gameServices.ASM.StageAction(actions[0]);
            _gameServices.ASM.Commit();
        
            var effects = _gameServices.Contexts.TurnContext.ExploreEffects;
            Assert.AreEqual(1, effects.Count);

            var resolvable = new CheckResolvable(zombieInstance, _valeros, CardUtils.SkillCheck(10, Skill.Arcane));
            var check = new CheckContext(resolvable);
            var dicePool = new DicePool();
            effects[0].ApplyTo(check, dicePool);
            Assert.AreEqual(0, dicePool.NumDice(4));
        }

        [Test]
        public void Cat_ExploreWithMagic()
        {
            _gameServices.Contexts.NewTurn(new TurnContext(_valeros));
        
            // Set up the location with a zombie card.
            var spellData = TestUtils.LoadCardData<CardData>("Enchant Weapon");
            var spellInstance = _gameServices.Cards.New(spellData);
            _valeros.Location.ShuffleIn(spellInstance, false);
        
            // Check that we can use the Cat to explore.
            var actions = _catInstance.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Discard, actions[0].ActionType);
        
            // Stage the explore action and explore.
            _gameServices.ASM.StageAction(actions[0]);
            _gameServices.ASM.Commit();
        
            var effects = _gameServices.Contexts.TurnContext.ExploreEffects;
            Assert.AreEqual(1, effects.Count);
        
            var resolvable = new CheckResolvable(spellInstance, _valeros, CardUtils.SkillCheck(10, Skill.Arcane));
            var check = new CheckContext(resolvable);
            var dicePool = new DicePool();
            effects[0].ApplyTo(check, dicePool);
            Assert.AreEqual(1, dicePool.NumDice(4));
        }
    }
}
