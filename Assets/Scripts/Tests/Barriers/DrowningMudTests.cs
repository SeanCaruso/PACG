using System.Linq;
using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Barriers
{
    public class DrowningMudTests : BaseTest
    {
        private PlayerCharacter _valeros;
        private CardInstance _longsword;
        private CardInstance _drowningMud;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _valeros = TestUtils.GetCharacter(GameServices, "Valeros");
            _longsword = TestUtils.GetCard(GameServices, "Longsword");
            _longsword.Owner = _valeros;
            _valeros.Reload(_longsword);
            
            _drowningMud = TestUtils.GetCard(GameServices, "Drowning Mud");
        }
        
        [Test]
        public void DrowningMud_Undefeated_Entangles()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_valeros, _drowningMud));
            _drowningMud.Logic.OnUndefeated(_drowningMud);
            
            Assert.IsTrue(_valeros.ActiveScourges.Contains(ScourgeType.Entangled));
        }
        
        [Test]
        public void DrowningMud_Undefeated_Exhausts()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_valeros, _drowningMud));
            _drowningMud.Logic.OnUndefeated(_drowningMud);
            
            Assert.IsTrue(_valeros.ActiveScourges.Contains(ScourgeType.Exhausted));
        }
        
        [Test]
        public void DrowningMud_Undefeated_Buries_Top_Card()
        {
            GameServices.Contexts.NewEncounter(new EncounterContext(_valeros, _drowningMud));
            _drowningMud.Logic.OnUndefeated(_drowningMud);
            
            Assert.AreEqual(0, _valeros.Deck.Count);
            Assert.AreEqual(1, _valeros.BuriedCards.Count);
            Assert.AreEqual(_longsword, _valeros.BuriedCards[0]);
        }
    }
}
