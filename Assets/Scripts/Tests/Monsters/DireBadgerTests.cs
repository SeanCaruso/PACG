using NUnit.Framework;
using PACG.Core;
using PACG.Gameplay;

namespace Tests.Monsters
{
    public class DireBadgerTests
    {
        private GameServices _gameServices;

        [SetUp]
        public void Setup()
        {
            _gameServices = TestUtils.CreateGameServices();
        }

        [TearDown]
        public void TearDown()
        {
            _gameServices.Contexts.EndCheck();
            _gameServices.Contexts.EndResolvable();
            _gameServices.Contexts.EndEncounter();
            _gameServices.Contexts.EndTurn();
        }

        [Test]
        public void DireBadger_ValidSkills()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Dire Badger");

            var check = _gameServices.Contexts.CheckContext;
            Assert.IsTrue(check.IsCombatValid);
            Assert.IsTrue(check.IsSkillValid);
            Assert.AreEqual(4, check.GetCurrentValidSkills().Count);
            Assert.AreEqual(11, check.GetDcForSkill(Skill.Strength));
            Assert.AreEqual(11, check.GetDcForSkill(Skill.Melee));
            Assert.AreEqual(6, check.GetDcForSkill(Skill.Perception));
            Assert.AreEqual(6, check.GetDcForSkill(Skill.Survival));
        }

        [Test]
        public void DireBadger_Damage_On_Combat_Defeat()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Dire Badger");

            var check = _gameServices.Contexts.CheckContext;
            check.Resolvable.CheckSteps[0].baseDC = 1;
            
            _gameServices.ASM.Commit();
            
            Assert.IsTrue(check.CheckResult.WasSuccess);
            
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable is DamageResolvable);
            
            var resolvable = _gameServices.Contexts.CurrentResolvable as DamageResolvable;
            Assert.IsTrue(resolvable?.Amount is > 0 and < 5);
        }

        [Test]
        public void DireBadger_No_Damage_On_Skill_Defeat()
        {
            TestUtils.SetupEncounter(_gameServices, "Valeros", "Dire Badger");

            var check = _gameServices.Contexts.CheckContext;
            check.Resolvable.CheckSteps[1].baseDC = 1;
            check.UsedSkill = Skill.Perception;
            
            _gameServices.ASM.Commit();
            
            Assert.IsTrue(check.CheckResult.WasSuccess);
            
            Assert.IsTrue(_gameServices.Contexts.CurrentResolvable == null);
            Assert.IsTrue(_gameServices.Contexts.CheckContext == null);
            Assert.IsTrue(_gameServices.Contexts.EncounterContext == null);
        }
    }
}
