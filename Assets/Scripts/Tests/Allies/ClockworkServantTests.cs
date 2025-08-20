using System.Collections.Generic;
using NUnit.Framework;
using PACG.Data;
using PACG.Gameplay;
using UnityEngine;

public class ClockworkServantTests
{
    private GameServices _gameServices;
    private CardData _cardData;
    private CardInstance _cardInstance;

    private CharacterData _valerosData;
    private PlayerCharacter _valeros;

    [SetUp]
    public void Setup()
    {
        _gameServices = TestUtils.CreateGameServices();
        _gameServices.Contexts.NewGame(new GameContext(1, _gameServices.Cards));
        
        _valerosData = TestUtils.LoadCharacterData("Valeros");
        _valeros = new PlayerCharacter(_valerosData, _gameServices.Logic.GetLogic<CharacterLogicBase>(_valerosData.characterName), _gameServices);
        _gameServices.Contexts.GameContext.SetPcLocation(_valeros, new Location(ScriptableObject.CreateInstance<LocationData>(), null, _gameServices));
        
        _cardData = TestUtils.LoadCardData("Clockwork Servant");
        _cardInstance = _gameServices.Cards.New(_cardData, _valeros);
        _cardInstance.CurrentLocation = CardLocation.Hand;
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
    public void ClockServ_CanRechargeVsInt()
    {
        // Set up a new encounter.
        _gameServices.Contexts.NewTurn(new TurnContext(_valeros));

        var encounterData = ScriptableObject.CreateInstance<BoonCardData>();
        encounterData.cardType = PF.CardType.Ally;
        encounterData.checkRequirement = new CheckRequirement
        {
            mode = CheckMode.Single,
            checkSteps = new List<CheckStep>
                { new() { category = CheckCategory.Skill, allowedSkills = new List<PF.Skill> { PF.Skill.Intelligence } } }
        };

        var encounterInstance = new CardInstance(encounterData, new ZombieLogic(_gameServices));

        _gameServices.Contexts.NewEncounter(new EncounterContext(_valeros, encounterInstance));

        // Start the encounter.
        _gameServices.GameFlow.StartPhase(new EncounterController(_valeros, encounterInstance, _gameServices),
            "Encounter");

        // Check that the game pauses when reaching the required check resolvable.
        Assert.IsNotNull(_gameServices.Contexts.CurrentResolvable);
        Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is SkillResolvable);

        // Check that the card has one recharge action.
        var actions = _cardInstance.GetAvailableActions();
        Assert.AreEqual(1, actions.Count);
        Assert.AreEqual(PF.ActionType.Recharge, actions[0].ActionType);
        
        // Check for +1d6.
        _cardInstance.Logic.Execute(_cardInstance, actions[0]);
        Assert.AreEqual(1, _gameServices.Contexts.CheckContext.DicePool.NumDice(6));
    }

    [Test]
    public void ClockServ_CanRechargeVsCraft()
    {
        // Set up a new encounter.
        _gameServices.Contexts.NewTurn(new TurnContext(_valeros));

        var encounterData = ScriptableObject.CreateInstance<BoonCardData>();
        encounterData.cardType = PF.CardType.Ally;
        encounterData.checkRequirement = new CheckRequirement
        {
            mode = CheckMode.Single,
            checkSteps = new List<CheckStep>
                { new() { category = CheckCategory.Skill, allowedSkills = new List<PF.Skill> { PF.Skill.Craft } } }
        };

        var encounterInstance = new CardInstance(encounterData, new ZombieLogic(_gameServices));

        _gameServices.Contexts.NewEncounter(new EncounterContext(_valeros, encounterInstance));

        // Start the encounter.
        _gameServices.GameFlow.StartPhase(new EncounterController(_valeros, encounterInstance, _gameServices),
            "Encounter");

        // Check that the game pauses when reaching the required check resolvable.
        Assert.IsNotNull(_gameServices.Contexts.CurrentResolvable);
        Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is SkillResolvable);

        // Check that the card has one recharge action.
        var actions = _cardInstance.GetAvailableActions();
        Assert.AreEqual(1, actions.Count);
        Assert.AreEqual(PF.ActionType.Recharge, actions[0].ActionType);
        
        // Check for +1d6.
        _cardInstance.Logic.Execute(_cardInstance, actions[0]);
        Assert.AreEqual(1, _gameServices.Contexts.CheckContext.DicePool.NumDice(6));
    }

    [Test]
    public void ClockServ_CanNotRecharge()
    {
        // Set up a new encounter with an Obstacle barrier.
        _gameServices.Contexts.NewTurn(new TurnContext(_valeros));

        var encounterData = ScriptableObject.CreateInstance<BoonCardData>();
        encounterData.cardType = PF.CardType.Item;
        encounterData.checkRequirement = new CheckRequirement
        {
            mode = CheckMode.Single,
            checkSteps = new List<CheckStep>
                { new() { category = CheckCategory.Skill, allowedSkills = new List<PF.Skill> { PF.Skill.Strength} } }
        };

        var encounterInstance = new CardInstance(encounterData, new ZombieLogic(_gameServices));

        _gameServices.Contexts.NewEncounter(new EncounterContext(_valeros, encounterInstance));

        // Start the encounter.
        _gameServices.GameFlow.StartPhase(new EncounterController(_valeros, encounterInstance, _gameServices),
            "Encounter");

        // Check that the game pauses when reaching the required check resolvable.
        Assert.IsNotNull(_gameServices.Contexts.CurrentResolvable);
        Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is SkillResolvable);

        // Check that the card doesn't have a recharge action.
        var actions = _cardInstance.GetAvailableActions();
        Assert.AreEqual(0, actions.Count);
    }

    [Test]
    public void ClockServ_TwoExploreOptions()
    {
        _gameServices.Contexts.NewTurn(new TurnContext(_valeros));
        
        // Set up the location with a zombie card.
        var zombieData = TestUtils.LoadCardData("Zombie");
        var zombieInstance = _gameServices.Cards.New(zombieData);
        _valeros.Location.ShuffleIn(zombieInstance, false);
        
        // Check that we can use the card to explore.
        var actions = _cardInstance.GetAvailableActions();
        Assert.AreEqual(2, actions.Count);
        Assert.AreEqual(PF.ActionType.Bury, actions[0].ActionType);
        Assert.AreEqual(PF.ActionType.Banish, actions[1].ActionType);
    }
}
