using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Scourges
{
    public class EntangledTests : BaseTest
    {
        private PlayerCharacter _valeros;
        private Location _location1;
        private Location _location2;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _valeros = TestUtils.GetCharacter(GameServices, "Valeros");
            _location1 = TestUtils.GetLocation(GameServices, "Caravan");
            _location2 = TestUtils.GetLocation(GameServices, "Caravan");
            
            var gameContext = new GameContext(1, GameServices.Cards);
            gameContext.AddLocation(_location1);
            gameContext.AddLocation(_location2);
            gameContext.SetPcLocation(_valeros, _location1);
            GameServices.Contexts.NewGame(gameContext);
        }
        
        [Test]
        public void Entangled_Unscourged_Doesnt_Prevent_Move()
        {
            GameServices.GameFlow.StartPhase(new StartTurnController(_valeros, GameServices), "Turn");
            Assert.IsTrue(GameServices.Contexts.TurnContext.CanMove);
        }
        
        [Test]
        public void Entangled_Scourged_Prevents_Move()
        {
            _valeros.AddScourge(ScourgeType.Entangled);
            GameServices.GameFlow.StartPhase(new StartTurnController(_valeros, GameServices), "Turn");
            Assert.IsFalse(GameServices.Contexts.TurnContext.CanMove);
        }

        [Test]
        public void Entangled_Unscourged_Doesnt_Prevent_Evasion()
        {
            GameServices.Contexts.NewTurn(new TurnContext(_valeros));
            
            var zombie = TestUtils.GetCard(GameServices, "Zombie");

            var encounter = new EncounterContext(_valeros, zombie);
            encounter.ExploreEffects.Add(new EvadeExploreEffect());
            GameServices.Contexts.NewEncounter(encounter);
            
            GameServices.GameFlow.StartPhase(new Encounter_EvasionProcessor(GameServices), "Evasion");
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            Assert.AreEqual("Evade?", resolvable.Prompt);
        }

        [Test]
        public void Entangled_Scourged_Prevents_Evasion()
        {
            _valeros.AddScourge(ScourgeType.Entangled);
            GameServices.Contexts.NewTurn(new TurnContext(_valeros));
            
            var zombie = TestUtils.GetCard(GameServices, "Zombie");

            var encounter = new EncounterContext(_valeros, zombie);
            encounter.ExploreEffects.Add(new EvadeExploreEffect());
            GameServices.Contexts.NewEncounter(encounter);
            
            GameServices.GameFlow.StartPhase(new Encounter_EvasionProcessor(GameServices), "Evasion");
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable == null);
        }

        [Test]
        public void Entangled_Location_Close_Removes()
        {
            _valeros.AddScourge(ScourgeType.Entangled);
            _valeros.Location.Close();
            
            Assert.IsTrue(_valeros.ActiveScourges.Count == 0);
        }
    }
}
