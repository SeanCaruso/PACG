using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.StoryBanes
{
    public class RescueTests : BaseTest
    {
        private CardInstance _rescue;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _rescue = TestUtils.GetCard(GameServices, "Rescue");
        }

        [Test]
        public void Rescue_Can_Recharge_Allies()
        {
            TestUtils.SetupEncounter(GameServices, Valeros, _rescue);

            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            Valeros.AddToHand(soldier);
            var sage = TestUtils.GetCard(GameServices, "Sage");
            Valeros.AddToHand(sage);
            var cat = TestUtils.GetCard(GameServices, "Cat");
            Valeros.AddToHand(cat);

            var soldierActions = _rescue.GetAdditionalActionsForCard(soldier);
            Assert.AreEqual(1, soldierActions.Count);
            var soldierAction = soldierActions[0] as PlayCardAction;
            Assert.IsNotNull(soldierAction);
            Assert.AreEqual(1, soldierAction.CheckModifier.AddedDice.Count);
            Assert.AreEqual(4, soldierAction.CheckModifier.AddedDice[0]);
            Assert.IsTrue(soldierAction.ActionData.TryGetValue("IsFreely", out var isFreely1));
            Assert.IsTrue((bool)isFreely1);
            GameServices.ASM.StageAction(soldierAction);
            Assert.AreEqual(1, GameServices.ASM.StagedActions.Count);
            
            var sageActions = _rescue.GetAdditionalActionsForCard(sage);
            Assert.AreEqual(1, sageActions.Count);
            var sageAction = sageActions[0] as PlayCardAction;
            Assert.IsNotNull(sageAction);
            Assert.AreEqual(1, sageAction.CheckModifier.AddedDice.Count);
            Assert.AreEqual(4, sageAction.CheckModifier.AddedDice[0]);
            Assert.IsTrue(sageAction.ActionData.TryGetValue("IsFreely", out var isFreely2));
            Assert.IsTrue((bool)isFreely2);
            GameServices.ASM.StageAction(sageAction);
            Assert.AreEqual(2, GameServices.ASM.StagedActions.Count);
            
            var catActions = _rescue.GetAdditionalActionsForCard(cat);
            Assert.AreEqual(1, catActions.Count);
            var catAction = catActions[0] as PlayCardAction;
            Assert.IsNotNull(catAction);
            Assert.AreEqual(1, catAction.CheckModifier.AddedDice.Count);
            Assert.AreEqual(4, catAction.CheckModifier.AddedDice[0]);
            Assert.IsTrue(catAction.ActionData.TryGetValue("IsFreely", out var isFreely3));
            Assert.IsTrue((bool)isFreely3);
            GameServices.ASM.StageAction(catAction);
            Assert.AreEqual(3, GameServices.ASM.StagedActions.Count);

            var dicePool = GameServices.Contexts.CheckContext.DicePool(GameServices.ASM.StagedActions);
            // Valeros should default to Melee (1d10 + 2) and have 3 ally dice.
            Assert.AreEqual("1d10 + 3d4 + 2", dicePool.ToString());
        }
    }
}
