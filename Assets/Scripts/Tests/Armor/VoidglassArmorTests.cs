using System.Linq;
using NUnit.Framework;
using PACG.Core;
using PACG.Gameplay;

namespace Tests.Armor
{
    public class VoidglassArmorTests : BaseTest
    {
        private CardInstance _voidglassArmor;
        
        [SetUp]
        public void SetUp()
        {
            _voidglassArmor = TestUtils.GetCard(GameServices, "Voidglass Armor");
        }
        
        [Test]
        public void VoidglassArmor_Can_Recharge_For_Any_Damage()
        {
            _voidglassArmor.Owner = Valeros;
            _voidglassArmor.CurrentLocation = CardLocation.Displayed;
            
            GameServices.Contexts.NewResolvable(new DamageResolvable(Valeros, 1, "Magic"));
            
            Assert.AreEqual(2, _voidglassArmor.GetAvailableActions().Count);
            Assert.AreEqual(ActionType.Recharge, _voidglassArmor.GetAvailableActions()[0].ActionType);
            Assert.AreEqual(ActionType.Bury, _voidglassArmor.GetAvailableActions()[1].ActionType);
            
            GameServices.ASM.StageAction(_voidglassArmor.GetAvailableActions()[0]);
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable.CanCommit(GameServices.ASM.StagedActions.ToList()));
        }
        
        [Test]
        public void VoidglassArmor_Can_Display_Then_Recharge_For_Any_Damage()
        {
            _voidglassArmor.Owner = Valeros;
            _voidglassArmor.CurrentLocation = CardLocation.Hand;
            
            GameServices.Contexts.NewResolvable(new DamageResolvable(Valeros, 1, "Special"));
            
            Assert.AreEqual(1, _voidglassArmor.GetAvailableActions().Count);
            Assert.AreEqual(ActionType.Display, _voidglassArmor.GetAvailableActions()[0].ActionType);
            
            GameServices.ASM.StageAction(_voidglassArmor.GetAvailableActions()[0]);
            
            Assert.AreEqual(2, _voidglassArmor.GetAvailableActions().Count);
            Assert.AreEqual(ActionType.Recharge, _voidglassArmor.GetAvailableActions()[0].ActionType);
            Assert.AreEqual(ActionType.Bury, _voidglassArmor.GetAvailableActions()[1].ActionType);
            
            GameServices.ASM.StageAction(_voidglassArmor.GetAvailableActions()[0]);
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable.CanCommit(GameServices.ASM.StagedActions.ToList()));
        }

        [Test]
        public void VoidglassArmor_Prompts_On_Mental_Damage_When_Displayed()
        {
            _voidglassArmor.Owner = Valeros;
            GameServices.Cards.MoveCard(_voidglassArmor, CardLocation.Displayed);
            
            GameServices.Contexts.NewResolvable(new DamageResolvable(Valeros, 3, "Mental"));
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            Assert.AreEqual(2, resolvable.Options.Count);
        }

        [Test]
        public void VoidglassArmor_Prompts_On_Mental_Damage_When_In_Hand()
        {
            _voidglassArmor.Owner = Valeros;
            GameServices.Cards.MoveCard(_voidglassArmor, CardLocation.Hand);
            
            GameServices.Contexts.NewResolvable(new DamageResolvable(Valeros, 3, "Mental"));
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            Assert.AreEqual(2, resolvable.Options.Count);
        }

        [Test]
        public void VoidglassArmor_Mental_Damage_Power_Allows_Recharge()
        {
            _voidglassArmor.Owner = Valeros;
            var longsword = TestUtils.GetCard(GameServices, "Longsword");
            Valeros.AddToHand(longsword);
            GameServices.Cards.MoveCard(_voidglassArmor, CardLocation.Hand);

            var damageResolvable = new DamageResolvable(Valeros, 1, "Mental");
            var processor = new NewResolvableProcessor(damageResolvable, GameServices);
            GameServices.GameFlow.StartPhase(processor, "Damage");
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            Assert.AreEqual(2, resolvable.Options.Count);
            
            resolvable.Options[0].Action.Invoke();
            Assert.AreEqual(CardLocation.Deck, _voidglassArmor.CurrentLocation);
            
            Assert.AreEqual(damageResolvable, GameServices.Contexts.CurrentResolvable);
            Assert.IsFalse(GameServices.Contexts.CurrentResolvable.CanCommit(GameServices.ASM.StagedActions.ToList()));
            
            var damageActions = GameServices.Contexts.CurrentResolvable.GetAdditionalActionsForCard(longsword);
            Assert.AreEqual(1, damageActions.Count);
            GameServices.ASM.StageAction(damageActions[0]);
            
            Assert.AreEqual(CardLocation.Deck, longsword.CurrentLocation);
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable.CanCommit(GameServices.ASM.StagedActions.ToList()));
        }

        [Test]
        public void VoidglassArmor_Prompts_On_Deck_Discard_When_Displayed()
        {
            _voidglassArmor.Owner = Valeros;
            GameServices.Cards.MoveCard(_voidglassArmor, CardLocation.Displayed);
            
            ScourgeRules.HandleWoundedDeckDiscard(Valeros, GameServices);
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            Assert.AreEqual(2, resolvable.Options.Count);
        }

        [Test]
        public void VoidglassArmor_Prompts_On_Deck_Discard_When_In_Hand()
        {
            _voidglassArmor.Owner = Valeros;
            GameServices.Cards.MoveCard(_voidglassArmor, CardLocation.Hand);
            
            ScourgeRules.HandleWoundedDeckDiscard(Valeros, GameServices);
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            Assert.AreEqual(2, resolvable.Options.Count);
        }

        [Test]
        public void VoidglassArmor_Recharge_Instead_Of_Deck_Discard()
        {
            _voidglassArmor.Owner = Valeros;
            Valeros.ShuffleIntoDeck(Longsword);
            GameServices.Cards.MoveCard(_voidglassArmor, CardLocation.Hand);
            
            ScourgeRules.HandleWoundedDeckDiscard(Valeros, GameServices);
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            Assert.AreEqual(2, resolvable.Options.Count);
            
            resolvable.Options[0].Action.Invoke();
            Assert.AreEqual(CardLocation.Deck, _voidglassArmor.CurrentLocation);
            Assert.AreEqual(CardLocation.Deck, Longsword.CurrentLocation);
        }
    }
}
