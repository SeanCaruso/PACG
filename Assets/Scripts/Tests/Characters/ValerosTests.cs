using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.Characters
{
    public class ValerosTests : BaseTest
    {
        [Test]
        public void Valeros_End_Turn_Power_Valid_Card_In_Hand()
        {
            Valeros.AddToHand(Longsword);

            var rechargePower = Valeros.GetEndOfTurnPower();
            Assert.IsNotNull(rechargePower);
            Assert.AreEqual(Valeros.CharacterData.Powers[1], rechargePower.Value);
        }
        
        [Test]
        public void Valeros_End_Turn_Power_Valid_Card_In_Discards()
        {
            Longsword.Owner = Valeros;
            GameServices.Cards.MoveCard(Longsword, CardLocation.Discard);

            var rechargePower = Valeros.GetEndOfTurnPower();
            Assert.IsNotNull(rechargePower);
            Assert.AreEqual(Valeros.CharacterData.Powers[1], rechargePower.Value);
        }
        
        [Test]
        public void Valeros_End_Turn_Power_No_Valid_Card_In_Hand()
        {
            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            Valeros.AddToHand(soldier);

            var rechargePower = Valeros.GetEndOfTurnPower();
            Assert.IsNull(rechargePower);
        }
        
        [Test]
        public void Valeros_End_Turn_Power_No_Valid_Card_In_Discards()
        {
            var soldier = TestUtils.GetCard(GameServices, "Soldier");
            soldier.Owner = Valeros;
            GameServices.Cards.MoveCard(soldier, CardLocation.Discard);

            var rechargePower = Valeros.GetEndOfTurnPower();
            Assert.IsNull(rechargePower);
        }
    }
}
