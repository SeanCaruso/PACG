using NUnit.Framework;
using PACG.Gameplay;

namespace Tests.StoryBanes
{
    public class DireWolfTests : BaseTest
    {
        [Test]
        public void Dire_Wolf_Prevents_Evasion()
        {
            GameServices.Contexts.NewTurn(new TurnContext(Valeros));
            
            var direWolf = TestUtils.GetCard(GameServices, "Dire Wolf");

            var encounter = new EncounterContext(Valeros, direWolf);
            encounter.ExploreEffects.Add(new EvadeExploreEffect());
            GameServices.Contexts.NewEncounter(encounter);
            
            GameServices.GameFlow.StartPhase(new Encounter_EvasionProcessor(GameServices), "Evasion");
            Assert.IsNull(GameServices.Contexts.CurrentResolvable);
        }
        
        [Test]
        public void Dire_Wolf_Adds_Damage()
        {
            var direWolf = TestUtils.GetCard(GameServices, "Dire Wolf");
            for (var i = 0; i < 100; i++)
            {
                GameServices.Contexts.NewEncounter(new EncounterContext(Valeros, direWolf));
                direWolf.Logic.OnEncounter();
                
                const int baseDamage = 1;
                var resolvable = new DamageResolvable(Valeros, baseDamage, GameServices);
                GameServices.Contexts.NewResolvable(resolvable);
                
                Assert.IsTrue(resolvable.Amount >= baseDamage + 1);
                Assert.IsTrue(resolvable.Amount <= baseDamage + 4);
            }
        }
    }
}
