using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PACG.Core;
using PACG.Data;
using PACG.Gameplay;
using UnityEngine;

namespace Tests.Allies
{
    public class FrogTests : BaseTest
    {
        private CardInstance _frog;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            _frog = TestUtils.GetCard(GameServices, "Frog");
            Ezren.AddToHand(_frog);
        }

        [Test]
        public void Frog_Cannot_Evade_Monster()
        {
            TestUtils.SetupEncounter(GameServices, Ezren, Zombie);
            Assert.AreEqual(0, _frog.GetAvailableActions().Count);
        }

        [Test]
        public void Frog_Evade_Obstacle()
        {
            var encounterData = ScriptableObject.CreateInstance<BaneCardData>();
            encounterData.cardType = CardType.Barrier;
            encounterData.traits = new List<string> { "Obstacle", "Other Trait" };

            var encounterInstance = new CardInstance(encounterData, null);
            
            TestUtils.SetupEncounter(GameServices, Ezren, encounterInstance);

            // Check that the game pauses when reaching an Evade Resolvable.
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is EvadeResolvable);

            // Check that the frog has one evade action.
            var actions = _frog.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Bury, actions[0].ActionType);

            // Stage and commit the evade action.
            GameServices.ASM.StageAction(actions[0]);
            GameServices.ASM.Commit();

            // Check that the encounter ends.
            Assert.IsTrue(GameServices.Contexts.EncounterContext == null);
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
        }

        [Test]
        public void Frog_Evade_Trap()
        {
            var encounterData = ScriptableObject.CreateInstance<BaneCardData>();
            encounterData.cardType = CardType.Barrier;
            encounterData.traits = new List<string> { "Trap", "Other Trait" };

            var encounterInstance = new CardInstance(encounterData, null);
            
            TestUtils.SetupEncounter(GameServices, Ezren, encounterInstance);

            // Check that the game pauses when reaching an Evade Resolvable.
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is EvadeResolvable);

            // Check that the frog has one evade action.
            var actions = _frog.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Bury, actions[0].ActionType);

            // Stage and commit the evade action.
            GameServices.ASM.StageAction(actions[0]);
            GameServices.ASM.Commit();

            // Check that the encounter ends.
            Assert.IsTrue(GameServices.Contexts.EncounterContext == null);
            Assert.IsTrue(GameServices.Contexts.CurrentResolvable is PlayerChoiceResolvable);
        }

        [Test]
        public void Frog_Explore_Ignores_First_Scourge()
        {
            GameServices.Contexts.NewGame(new GameContext(1, GameServices.Cards));
            var caravan = TestUtils.GetLocation(GameServices, "Caravan");
            GameServices.Contexts.GameContext.SetPcLocation(Ezren, caravan);
            
            GameServices.Contexts.NewTurn(new TurnContext(Ezren));
            Ezren.Location.ShuffleIn(Longsword, true);
            
            var actions = _frog.GetAvailableActions();
            Assert.AreEqual(1, actions.Count);
            Assert.AreEqual(ActionType.Discard, actions[0].ActionType);
            
            _frog.Logic.OnCommit(actions[0]);
            Assert.AreEqual(1, GameServices.Contexts.TurnContext.ExploreEffects.Count);
            Assert.IsTrue(GameServices.Contexts.TurnContext.ExploreEffects[0] is ScourgeImmunityExploreEffect);
            
            Ezren.AddScourge(ScourgeType.Entangled);
            Assert.AreEqual(0, Ezren.ActiveScourges.Count);
            
            Ezren.AddScourge(ScourgeType.Wounded);
            Assert.AreEqual(1, Ezren.ActiveScourges.Count);
            Assert.AreEqual(ScourgeType.Wounded, Ezren.ActiveScourges.First());
        }
    }
}
