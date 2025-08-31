using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Spells
{
    public class DeflectTests : BaseTest
    {
        private PlayerCharacter _ezren;
        private CardInstance _deflect;
        private Location _caravan;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _ezren = TestUtils.GetCharacter(GameServices, "Ezren");
            _deflect = TestUtils.GetCard(GameServices, "Deflect");
            _ezren.AddToHand(_deflect);

            GameServices.Contexts.NewGame(new GameContext(1, GameServices.Cards));

            _caravan = TestUtils.GetLocation(GameServices, "Caravan");
            GameServices.Contexts.GameContext.SetPcLocation(_ezren, _caravan);
        }

        [Test]
        public void Deflect_Reduces_Four_Combat_Damage_To_Owner()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, Zombie));
            const int baseDamage = 4;
            GameServices.Contexts.NewResolvable(new DamageResolvable(_ezren, baseDamage, GameServices));

            var actions = _deflect.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);

            Assert.IsTrue(GameServices.Contexts.CurrentResolvable.CanCommit(actions));
        }

        [Test]
        public void Deflect_Not_Usable_Non_Combat_Damage_To_Owner()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, Zombie));
            const int baseDamage = 5;
            GameServices.Contexts.NewResolvable(new DamageResolvable(_ezren, baseDamage, GameServices, "Magic"));

            var actions = _deflect.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Deflect_Reduces_Four_Combat_Damage_To_Other_Local_Character()
        {
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, _caravan);

            GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, Zombie));
            const int baseDamage = 4;
            GameServices.Contexts.NewResolvable(new DamageResolvable(Valeros, baseDamage, GameServices));

            var actions = _deflect.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);

            Assert.IsTrue(GameServices.Contexts.CurrentResolvable.CanCommit(actions));
        }

        [Test]
        public void Deflect_Not_Usable_Non_Combat_Damage_To_Other_Local_Character()
        {
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, _caravan);

            GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, Zombie));
            const int baseDamage = 5;
            GameServices.Contexts.NewResolvable(new DamageResolvable(Valeros, baseDamage, GameServices, "Magic"));

            var actions = _deflect.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Deflect_Not_Usable_Combat_Damage_To_Distant_Character()
        {
            var campsite = TestUtils.GetLocation(GameServices, "Campsite");
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, campsite);

            GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, Zombie));
            const int baseDamage = 5;
            GameServices.Contexts.NewResolvable(new DamageResolvable(Valeros, baseDamage, GameServices, "Magic"));

            var actions = _deflect.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }
    }
}
