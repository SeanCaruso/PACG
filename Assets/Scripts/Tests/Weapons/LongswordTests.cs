using NUnit.Framework;
using PACG.Data;
using PACG.Gameplay;

public class LongswordTests
{
    private GameServices _gameServices;
    private CardData _longswordData;
    private CardInstance _longswordInstance;

    private CharacterData _valerosData;
    private PlayerCharacter _valeros;

    [SetUp]
    public void Setup()
    {
        _gameServices = TestUtils.CreateGameServices();
        
        _valerosData = TestUtils.LoadCharacterData("Valeros");
        _valeros = new PlayerCharacter(_valerosData, _gameServices.Logic.GetLogic<CharacterLogicBase>(_valerosData.CharacterName), _gameServices);
        
        _longswordData = TestUtils.LoadCardData("Longsword");
        _longswordInstance = _gameServices.Cards.New(_longswordData, _valeros);
        _longswordInstance.CurrentLocation = CardLocation.Hand;
        
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
    public void LongswordLogic_SetupWasSuccessful()
    {
        Assert.IsNotNull(_longswordData);
        Assert.AreEqual("Longsword", _longswordInstance.Data.cardName);
        Assert.AreEqual(PF.CardType.Weapon, _longswordInstance.Data.cardType);
        
        Assert.IsNotNull(_valerosData);
        Assert.AreEqual("Valeros", _valeros.CharacterData.CharacterName);
        Assert.IsTrue(_valeros.IsProficient(_longswordInstance.Data));
    }

    [Test]
    public void Longsword_Combat_ProficientActions()
    {
        TestUtils.SetupCombatCheck(_gameServices, _valeros);

        // Before staging, a proficient PC has two actions.
        var actions = _longswordInstance.GetAvailableActions();
        Assert.AreEqual(2, actions.Count);
        Assert.AreEqual(PF.ActionType.Reveal, actions[0].ActionType);
        Assert.AreEqual(PF.ActionType.Reload, actions[1].ActionType);
        
        // After staging, a proficient PC has one action.
        _gameServices.ASM.StageAction(actions[0]);
        actions = _longswordInstance.GetAvailableActions();
        Assert.AreEqual(1, actions.Count);
        Assert.AreEqual(PF.ActionType.Reload, actions[0].ActionType);
    }

    [Test]
    public void Longsword_Reveal_Then_Reload()
    {
        TestUtils.SetupCombatCheck(_gameServices, _valeros);
        var actions = _longswordInstance.GetAvailableActions();
        _gameServices.ASM.StageAction(actions[0]);

        var stagedPool = _gameServices.ASM.GetStagedDicePool();
        Assert.AreEqual(0, stagedPool.NumDice(12));
        Assert.AreEqual(1, stagedPool.NumDice(10));
        Assert.AreEqual(1, stagedPool.NumDice(8));
        Assert.AreEqual(0, stagedPool.NumDice(6));
        Assert.AreEqual(0, stagedPool.NumDice(4));
        
        actions = _longswordInstance.GetAvailableActions();
        _gameServices.ASM.StageAction(actions[0]);
        stagedPool = _gameServices.ASM.GetStagedDicePool();
        Assert.AreEqual(0, stagedPool.NumDice(12));
        Assert.AreEqual(1, stagedPool.NumDice(10));
        Assert.AreEqual(1, stagedPool.NumDice(8));
        Assert.AreEqual(0, stagedPool.NumDice(6));
        Assert.AreEqual(1, stagedPool.NumDice(4));
    }

    [Test]
    public void Longsword_Reload()
    {
        TestUtils.SetupCombatCheck(_gameServices, _valeros);
        var actions = _longswordInstance.GetAvailableActions();
        _gameServices.ASM.StageAction(actions[1]);
        
        var stagedPool = _gameServices.ASM.GetStagedDicePool();
        Assert.AreEqual(0, stagedPool.NumDice(12));
        Assert.AreEqual(1, stagedPool.NumDice(10));
        Assert.AreEqual(1, stagedPool.NumDice(8));
        Assert.AreEqual(0, stagedPool.NumDice(6));
        Assert.AreEqual(1, stagedPool.NumDice(4));
    }
}
