using System.Linq;
using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Scourges
{
    public class ExhaustedTests : BaseTest
    {
        [Test]
        public void Exhausted_Limits_To_One_Boon()
        {
            Valeros.AddScourge(ScourgeType.Exhausted);
            Valeros.AddToHand(Longsword);
            
            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            Valeros.AddToHand(soldier);

            var resolvable = new CheckResolvable(Zombie, Valeros, Zombie.Data.checkRequirement);
            GameServices.Contexts.NewResolvable(resolvable);
            
            Assert.AreEqual(2, Longsword.GetAvailableActions().Count);
            GameServices.ASM.StageAction(Longsword.GetAvailableActions()[0]);
            
            Assert.AreEqual(0, soldier.GetAvailableActions().Count);
        }
        
        [Test]
        public void Exhausted_Doesnt_Limit_Same_Boon()
        {
            Valeros.AddScourge(ScourgeType.Exhausted);
            Valeros.AddToHand(Longsword);

            var resolvable = new CheckResolvable(Zombie, Valeros, Zombie.Data.checkRequirement);
            GameServices.Contexts.NewResolvable(resolvable);
            
            Assert.AreEqual(2, Longsword.GetAvailableActions().Count);
            GameServices.ASM.StageAction(Longsword.GetAvailableActions()[0]);
            
            Assert.AreEqual(1, Longsword.GetAvailableActions().Count);
        }

        [Test]
        public void Exhausted_Removal_Prompt_On_Turn_Start()
        {
            Valeros.AddScourge(ScourgeType.Exhausted);
            GameServices.Contexts.NewTurn(new TurnContext(Valeros));
            
            GameServices.GameFlow.StartPhase(new Turn_StartTurnProcessor(GameServices), "Turn");
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
        }

        [Test]
        public void Exhausted_Removed()
        {
            Valeros.AddScourge(ScourgeType.Exhausted);
            ScourgeRules.PromptForExhaustedRemoval(Valeros, GameServices);
            
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
            
            var resolvable = (PlayerChoiceResolvable) GameServices.Contexts.CurrentResolvable;
            resolvable.Options[1].Action.Invoke();
            Assert.IsTrue(Valeros.ActiveScourges.Count == 1);
            Assert.IsTrue(Valeros.ActiveScourges.Contains(ScourgeType.Exhausted));
            
            resolvable.Options[0].Action.Invoke();
            Assert.IsTrue(Valeros.ActiveScourges.Count == 0);
        }
    }
}
