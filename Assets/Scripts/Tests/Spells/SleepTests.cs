using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Spells
{
    public class SleepTests : BaseTest
    {
        private PlayerCharacter _ezren;
        private CardInstance _sleep;
        private CardInstance _direBadger;
        private Location _caravan;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _ezren = TestUtils.GetCharacter(GameServices, "Ezren");
            _sleep = TestUtils.GetCard(GameServices, "Sleep");
            _ezren.AddToHand(_sleep);
            
            _direBadger = TestUtils.GetCard(GameServices, "Dire Badger");
            _direBadger.CurrentLocation = CardLocation.Deck;
            _caravan = TestUtils.GetLocation(GameServices, "Caravan");
            
            GameServices.Contexts.NewGame(new GameContext(1, GameServices.Cards));
            GameServices.Contexts.GameContext.SetPcLocation(_ezren, _caravan);
        }

        [Test]
        public void Sleep_To_Evade_Own_Monster_Encounter()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, _direBadger));

            var processor = new Encounter_EvasionProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is EvadeResolvable);
            Assert.AreEqual(1, _sleep.GetAvailableActions().Count);
            
            GameServices.Contexts.CurrentResolvable.Resolve();
            Assert.AreEqual(1, _caravan.Count);
            Assert.AreEqual(_direBadger, _caravan.ExamineTop(1)[0]);
        }

        [Test]
        public void Sleep_To_Evade_Local_Monster_Encounter()
        {
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, _caravan);
            GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, _direBadger));

            var processor = new Encounter_EvasionProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is EvadeResolvable);
            Assert.AreEqual(1, _sleep.GetAvailableActions().Count);
            
            GameServices.Contexts.CurrentResolvable.Resolve();
            Assert.AreEqual(1, _caravan.Count);
            Assert.AreEqual(_direBadger, _caravan.ExamineTop(1)[0]);
        }

        [Test]
        public void Sleep_Cannot_Evade_Distant_Monster_Encounter()
        {
            var campsite = TestUtils.GetLocation(GameServices, "Campsite");
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, campsite);
            GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, _direBadger));

            var processor = new Encounter_EvasionProcessor(GameServices);
            processor.Execute();
            
            Assert.IsNull(GameServices.Contexts.CurrentResolvable);
            Assert.AreEqual(0, _sleep.GetAvailableActions().Count);
        }

        [Test]
        public void Sleep_Cannot_Evade_Immune_Monster_Encounter()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, Zombie));

            var processor = new Encounter_EvasionProcessor(GameServices);
            processor.Execute();
            
            Assert.IsNull(GameServices.Contexts.CurrentResolvable);
            Assert.AreEqual(0, _sleep.GetAvailableActions().Count);
        }

        [Test]
        public void Sleep_Cannot_Evade_Unevadable_Monster_Encounter()
        {
            var direWolf = TestUtils.GetCard(GameServices, "Dire Wolf");
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, direWolf));

            var processor = new Encounter_EvasionProcessor(GameServices);
            processor.Execute();
            
            Assert.IsNull(GameServices.Contexts.CurrentResolvable);
            Assert.AreEqual(0, _sleep.GetAvailableActions().Count);
        }

        [Test]
        public void Sleep_Adds_Bonus_To_Own_Monster_Check()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, _direBadger));

            var processor = new AttemptChecksProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is CheckResolvable);
            Assert.AreEqual(1, _sleep.GetAvailableActions().Count);
            
            var modifier = (_sleep.GetAvailableActions()[0] as PlayCardAction)?.CheckModifier;
            Assert.IsNotNull(modifier);
            Assert.AreEqual(1, modifier.AddedDice.Count);
            Assert.AreEqual(6, modifier.AddedDice[0]);
        }

        [Test]
        public void Sleep_Adds_Bonus_To_Own_Ally_Check()
        {
            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, soldier));

            var processor = new AttemptChecksProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is CheckResolvable);
            Assert.AreEqual(1, _sleep.GetAvailableActions().Count);
            
            var modifier = (_sleep.GetAvailableActions()[0] as PlayCardAction)?.CheckModifier;
            Assert.IsNotNull(modifier);
            Assert.AreEqual(1, modifier.AddedDice.Count);
            Assert.AreEqual(6, modifier.AddedDice[0]);
        }

        [Test]
        public void Sleep_Adds_Bonus_To_Local_Monster_Check()
        {
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, _caravan);
            GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, _direBadger));

            var processor = new AttemptChecksProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is CheckResolvable);
            Assert.AreEqual(1, _sleep.GetAvailableActions().Count);
            
            var modifier = (_sleep.GetAvailableActions()[0] as PlayCardAction)?.CheckModifier;
            Assert.IsNotNull(modifier);
            Assert.AreEqual(1, modifier.AddedDice.Count);
            Assert.AreEqual(6, modifier.AddedDice[0]);
        }

        [Test]
        public void Sleep_Adds_Bonus_To_Local_Ally_Check()
        {
            GameServices.Contexts.GameContext.SetPcLocation(Valeros, _caravan);
            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, soldier));

            var processor = new AttemptChecksProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is CheckResolvable);
            Assert.AreEqual(1, _sleep.GetAvailableActions().Count);
            
            var modifier = (_sleep.GetAvailableActions()[0] as PlayCardAction)?.CheckModifier;
            Assert.AreEqual(1, modifier?.AddedDice.Count);
            Assert.AreEqual(6, modifier.AddedDice[0]);
        }

        [Test]
        public void Sleep_Does_Not_Add_Bonus_To_Own_Immune_Monster_Check()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_ezren, Zombie));

            var processor = new AttemptChecksProcessor(GameServices);
            processor.Execute();
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is CheckResolvable);
            Assert.AreEqual(0, _sleep.GetAvailableActions().Count);
        }
    }
}
