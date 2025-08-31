using NUnit.Framework;
using PACG.Core;
using PACG.Data;
using PACG.Gameplay;

namespace Tests.Weapons
{
    public class LongswordTests : BaseTest
    {
    
        [Test]
        public void LongswordLogic_SetupWasSuccessful()
        {
            Assert.IsNotNull(Longsword.Data);
            Assert.AreEqual("Longsword", Longsword.Data.cardName);
            Assert.AreEqual(CardType.Weapon, Longsword.Data.cardType);
        
            Assert.IsNotNull(Valeros.CharacterData);
            Assert.AreEqual("Valeros", Valeros.CharacterData.CharacterName);
            Assert.IsTrue(Valeros.IsProficient(Longsword.Data));
        }

        [Test]
        public void Longsword_Combat_ProficientActions()
        {
            TestUtils.SetupEncounter(GameServices, "Valeros", "Zombie");
            var longsword = TestUtils.GetCard(GameServices, "Longsword");
            GameServices.Contexts.EncounterContext.Character.AddToHand(longsword);

            // Before staging, a proficient PC has two actions.
            var actions = longsword.GetAvailableActions();
            Assert.AreEqual(2, actions.Count);
            Assert.AreEqual(ActionType.Reveal, actions[0].ActionType);
            Assert.AreEqual(ActionType.Reload, actions[1].ActionType);
        
            // After staging, a proficient PC has one action.
            GameServices.ASM.StageAction(actions[0]);
            actions = longsword.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Reload, actions[0].ActionType);
        }

        [Test]
        public void Longsword_Reveal_Then_Reload()
        {
            TestUtils.SetupEncounter(GameServices, "Valeros", "Zombie");
            GameServices.Contexts.EncounterContext.Character.AddToHand(Longsword);
            
            var actions = Longsword.GetAvailableActions();
            GameServices.ASM.StageAction(actions[0]);

            var stagedPool = GameServices.ASM.GetStagedDicePool();
            Assert.AreEqual(0, stagedPool.NumDice(12));
            Assert.AreEqual(1, stagedPool.NumDice(10));
            Assert.AreEqual(1, stagedPool.NumDice(8));
            Assert.AreEqual(0, stagedPool.NumDice(6));
            Assert.AreEqual(0, stagedPool.NumDice(4));
        
            actions = Longsword.GetAvailableActions();
            GameServices.ASM.StageAction(actions[0]);
            stagedPool = GameServices.ASM.GetStagedDicePool();
            Assert.AreEqual(0, stagedPool.NumDice(12));
            Assert.AreEqual(1, stagedPool.NumDice(10));
            Assert.AreEqual(1, stagedPool.NumDice(8));
            Assert.AreEqual(0, stagedPool.NumDice(6));
            Assert.AreEqual(1, stagedPool.NumDice(4));
        }

        [Test]
        public void Longsword_Reload()
        {
            TestUtils.SetupEncounter(GameServices, "Valeros", "Zombie");
            GameServices.Contexts.EncounterContext.Character.AddToHand(Longsword);
            
            var actions = Longsword.GetAvailableActions();
            GameServices.ASM.StageAction(actions[1]);
        
            var stagedPool = GameServices.ASM.GetStagedDicePool();
            Assert.AreEqual(0, stagedPool.NumDice(12));
            Assert.AreEqual(1, stagedPool.NumDice(10));
            Assert.AreEqual(1, stagedPool.NumDice(8));
            Assert.AreEqual(0, stagedPool.NumDice(6));
            Assert.AreEqual(1, stagedPool.NumDice(4));
        }

        [Test]
        public void Longsword_Adds_Traits()
        {
            TestUtils.SetupEncounter(GameServices, "Valeros", "Zombie");
            GameServices.Contexts.EncounterContext.Character.AddToHand(Longsword);
        
            var actions = Longsword.GetAvailableActions();
            GameServices.ASM.StageAction(actions[0]);

            foreach (var trait in Longsword.Traits)
            {
                Assert.IsTrue(GameServices.Contexts.CheckContext.Invokes(trait));
            }
        }

        [Test]
        public void Longsword_Not_Usable_During_Damage()
        {
            Valeros.AddToHand(Longsword);
            
            var damage = new DamageResolvable(Valeros, 1, GameServices, "Magic");
            GameServices.Contexts.NewResolvable(damage);
            
            var actions = Longsword.GetAvailableActions();
            Assert.AreEqual(0, actions.Count);
        }
    }
}
