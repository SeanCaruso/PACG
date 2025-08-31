using NUnit.Framework;
using PACG.Core;
using PACG.Gameplay;

namespace Tests.Allies
{
    public class SageTests : BaseTest
    {
        private CardInstance _sage;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _sage = TestUtils.GetCard(GameServices, "Sage");
            Ezren.AddToHand(_sage);
        }

        [Test]
        public void Sage_No_Actions_Combat()
        {
            TestUtils.SetupEncounter(GameServices, Ezren, Zombie);
            var frostbite = TestUtils.GetCard(GameServices, "Frostbite");
            Ezren.AddToHand(frostbite);
            GameServices.ASM.StageAction(frostbite.GetAvailableActions()[0]);
            
            Assert.AreEqual(Skill.Arcane, GameServices.Contexts.CheckContext.UsedSkill);
            
            var actions = _sage.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Sage_No_Actions_Non_Arcane_Knowledge_Skill()
        {
            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            TestUtils.SetupEncounter(GameServices, Ezren, soldier);
            
            Assert.IsTrue(GameServices.Contexts.CheckContext.IsSkillValid);
            
            var actions = _sage.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void Sage_Adds_To_Own_Arcane_Skill_Check()
        {
            var frostbite = TestUtils.GetCard(GameServices, "Frostbite");
            TestUtils.SetupEncounter(GameServices, Ezren, frostbite);
            
            var actions = _sage.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Recharge, actions[0].ActionType);
            
            var mod = (actions[0] as PlayCardAction)?.CheckModifier;
            Assert.AreEqual(1, mod?.AddedDice.Count);
            Assert.AreEqual(6, mod.AddedDice[0]);
        }

        [Test]
        public void Sage_Adds_To_Own_Knowledge_Skill_Check()
        {
            var codex = TestUtils.GetCard(GameServices, "Codex");
            TestUtils.SetupEncounter(GameServices, Ezren, codex);
            
            var actions = _sage.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Recharge, actions[0].ActionType);
            
            var mod = (actions[0] as PlayCardAction)?.CheckModifier;
            Assert.AreEqual(1, mod?.AddedDice.Count);
            Assert.AreEqual(6, mod.AddedDice[0]);
        }

        [Test]
        public void Sage_Adds_To_Local_Arcane_Skill_Check()
        {
            var frostbite = TestUtils.GetCard(GameServices, "Frostbite");
            TestUtils.SetupEncounter(GameServices, Valeros, frostbite);
            GameServices.Contexts.GameContext.SetPcLocation(Ezren, Valeros.Location);
            
            var actions = _sage.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Recharge, actions[0].ActionType);
            
            var mod = (actions[0] as PlayCardAction)?.CheckModifier;
            Assert.IsNotNull( mod);
            Assert.AreEqual(1, mod.AddedDice.Count);
            Assert.AreEqual(6, mod.AddedDice[0]);
        }

        [Test]
        public void Sage_Adds_To_Local_Knowledge_Skill_Check()
        {
            var codex = TestUtils.GetCard(GameServices, "Codex");
            TestUtils.SetupEncounter(GameServices, Valeros, codex);
            GameServices.Contexts.GameContext.SetPcLocation(Ezren, Valeros.Location);
            
            var actions = _sage.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Recharge, actions[0].ActionType);
            
            var mod = (actions[0] as PlayCardAction)?.CheckModifier;
            Assert.IsNotNull( mod);
            Assert.AreEqual(1, mod.AddedDice.Count);
            Assert.AreEqual(6, mod.AddedDice[0]);
        }
    }
}
