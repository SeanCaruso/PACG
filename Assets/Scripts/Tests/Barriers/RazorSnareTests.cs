using System.Linq;
using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Barriers
{
    public class RazorSnareTests : BaseTest
    {
        [Test]
        public void RazorSnare_Undefeated_Entangles()
        {
            TestUtils.SetupEncounter(GameServices, "Valeros", "Razor Snare");

            var check = GameServices.Contexts.CheckContext;
            check.Resolvable.CheckSteps[0].baseDC = 99;
            
            GameServices.ASM.Commit();
            
            Assert.IsFalse(check.CheckResult.WasSuccess);
            Assert.IsTrue(check.Character.ActiveScourges.Contains(ScourgeType.Entangled));
        }
        
        [Test]
        public void RazorSnare_Undefeated_Wounds()
        {
            TestUtils.SetupEncounter(GameServices, "Valeros", "Razor Snare");

            var check = GameServices.Contexts.CheckContext;
            check.Resolvable.CheckSteps[0].baseDC = 99;
            
            GameServices.ASM.Commit();
            
            Assert.IsFalse(check.CheckResult.WasSuccess);
            Assert.IsTrue(check.Character.ActiveScourges.Contains(ScourgeType.Wounded));
        }
        
        [Test]
        public void RazorSnare_Undefeated_EndsTurn()
        {
            var valeros = TestUtils.GetCharacter(GameServices, "Valeros");
            GameServices.Contexts.NewTurn(new TurnContext(valeros));
            
            var card = TestUtils.GetCard(GameServices, "Razor Snare");
            card.Logic.OnUndefeated(card);
            
            Assert.IsTrue(GameServices.Contexts.TurnContext.ForceEndTurn);
        }
    }
}
