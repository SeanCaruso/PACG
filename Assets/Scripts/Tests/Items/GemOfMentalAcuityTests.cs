using NUnit.Framework;
using PACG.Data;
using PACG.Gameplay;

namespace Tests.Items
{
    public class GemOfMentalAcuityTests
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

            _valerosData = TestUtils.LoadCharacterData("Valeros");
            _valeros = new PlayerCharacter(
                _valerosData,
                _gameServices.Logic.GetLogic<CharacterLogicBase>(_valerosData.CharacterName),
                _gameServices
            );

            _cardData = TestUtils.LoadCardData("Gem of Mental Acuity");
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
        public void GemMentalAcuity_Combat_NoActions()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Zombie");
            var actions = _cardInstance.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void GemMentalAcuity_NonCombat_OneAction()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Soldier");
            _gameServices.Contexts.TurnContext.Character.AddToHand(_cardInstance);

            var actions = _cardInstance.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
        }

        [Test]
        public void GemMentalAcuity_Valeros_D6()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Soldier");
            _gameServices.Contexts.TurnContext.Character.AddToHand(_cardInstance);

            // Default should be Melee.
            var dicePool = _gameServices.ASM.GetStagedDicePool();
            Assert.AreEqual("1d10 + 2", dicePool.ToString());

            var actions = _cardInstance.GetAvailableActions();
            _gameServices.ASM.StageAction(actions[0]);
            dicePool = _gameServices.ASM.GetStagedDicePool();
            Assert.AreEqual("1d6 + 2", dicePool.ToString());
        }
    }
}
