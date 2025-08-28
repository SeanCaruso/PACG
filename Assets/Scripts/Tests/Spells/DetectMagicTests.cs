using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Spells
{
    public class DetectMagicTests : BaseTest
    {
        private PlayerCharacter _ezren;
        private CardInstance _detectMagic;
        private Location _caravan;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _ezren = TestUtils.GetCharacter(GameServices, "Ezren");
            _detectMagic = TestUtils.GetCard(GameServices, "Detect Magic");
            _ezren.AddToHand(_detectMagic);
            
            GameServices.Contexts.NewGame(new GameContext(1, GameServices.Cards));
            
            _caravan = TestUtils.GetLocation(GameServices, "Caravan");
            GameServices.Contexts.GameContext.SetPcLocation(_ezren, _caravan);
        }

        [Test]
        public void Detect_Magic_Allows_Explore_For_Magic_Card_On_Owners_Turn()
        {
            GameServices.Contexts.NewTurn(new TurnContext(_ezren));
            
            var frostbite = TestUtils.GetCard(GameServices, "Frostbite");
            _caravan.ShuffleIn(frostbite, true);
            
            var actions = _detectMagic.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            _detectMagic.Logic.OnCommit(actions[0]);
            GameServices.GameFlow.Process();
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is ExamineResolvable);
            
            var resolvable = GameServices.Contexts.CurrentResolvable as ExamineResolvable;
            Assert.IsNotNull(resolvable);

            var processor = resolvable.CreateProcessor(GameServices);
            Assert.IsTrue(processor is NewResolvableProcessor);
            
            processor.Execute();
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var exploreResolvable = GameServices.Contexts.CurrentResolvable as PlayerChoiceResolvable;
            Assert.IsNotNull(exploreResolvable);
            Assert.AreEqual("Explore?", exploreResolvable.Prompt);
        }

        [Test]
        public void Detect_Magic_Allows_Shuffle_For_Non_Magic_Card()
        {
            _caravan.ShuffleIn(Zombie, true);
            
            var actions = _detectMagic.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            _detectMagic.Logic.OnCommit(actions[0]);
            GameServices.GameFlow.Process();
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is ExamineResolvable);
            
            var resolvable = GameServices.Contexts.CurrentResolvable as ExamineResolvable;
            Assert.IsNotNull(resolvable);

            var processor = resolvable.CreateProcessor(GameServices);
            Assert.IsTrue(processor is NewResolvableProcessor);
            
            processor.Execute();
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var shuffleResolvable = GameServices.Contexts.CurrentResolvable as PlayerChoiceResolvable;
            Assert.IsNotNull(shuffleResolvable);
            Assert.AreEqual("Shuffle?", shuffleResolvable.Prompt);
        }

        [Test]
        public void Detect_Magic_Does_Not_Allow_Explore_Or_Shuffle_For_Magic_Card_On_Another_Turn()
        {
            GameServices.Contexts.NewTurn(new TurnContext(Valeros));
            
            var frostbite = TestUtils.GetCard(GameServices, "Frostbite");
            _caravan.ShuffleIn(frostbite, true);
            
            var actions = _detectMagic.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            
            _detectMagic.Logic.OnCommit(actions[0]);
            GameServices.GameFlow.Process();
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is ExamineResolvable);
            
            var resolvable = GameServices.Contexts.CurrentResolvable as ExamineResolvable;
            Assert.IsNotNull(resolvable);

            var processor = resolvable.CreateProcessor(GameServices);
            Assert.IsNull(processor);
        }
    }
}
