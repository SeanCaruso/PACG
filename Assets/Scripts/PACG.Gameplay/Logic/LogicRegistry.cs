using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PACG.Gameplay
{
    public class LogicRegistry
    {
        private readonly Dictionary<string, IEncounterLogic> encounterLogicMap = new();
        private readonly Dictionary<string, IPlayableLogic> playableLogicMap = new();

        public LogicRegistry(ContextManager contexts)
        {
            RegisterAllLogic(contexts);
        }

        private void RegisterAllLogic(ContextManager contexts)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract) continue;

                // Check for Encounter logic.
                if (typeof(IEncounterLogic).IsAssignableFrom(type))
                {
                    var attribute = type.GetCustomAttribute<EncounterLogicForAttribute>();
                    if (attribute == null)
                        continue;

                    try
                    {
                        var instance = Activator.CreateInstance(type, contexts, this) as IEncounterLogic;
                        encounterLogicMap[attribute.CardID] = instance;
                    }
                    catch (MissingMethodException ex)
                    {
                        Debug.LogError($"Failed to create {type.Name}: Make sure CreateInstance has the correct constructor signature for IEncounterLogic. Exception: {ex.Message}");
                        throw; // Re-throw to make it obvious something is wrong
                    }

                }

                // Check for Playable logic.
                if (typeof(IPlayableLogic).IsAssignableFrom(type))
                {
                    var attribute = type.GetCustomAttribute<PlayableLogicForAttribute>();
                    if (attribute == null)
                        continue;
                    
                    try
                    {
                        var instance = Activator.CreateInstance(type, contexts, this) as IPlayableLogic;
                        playableLogicMap[attribute.CardID] = instance;
                    }
                    catch (MissingMethodException ex)
                    {
                        Debug.LogError($"Failed to create {type.Name}: Make sure CreateInstance has the correct constructor signature for IPlayableLogic. Exception: {ex.Message}");
                        throw; // Re-throw to make it obvious something is wrong
                    }
                }
            }

            Debug.Log($"Registered {encounterLogicMap.Count} Encounter logics and {playableLogicMap.Count} Playable logics.");
        }

        // Public getters for logic types.
        public IEncounterLogic GetEncounterLogic(CardInstance card)
        {
            encounterLogicMap.TryGetValue(card.Data.cardID, out var logic);
            return logic;
        }

        public IPlayableLogic GetPlayableLogic(CardInstance card)
        {
            playableLogicMap.TryGetValue(card.Data.cardID, out var logic);
            if (logic != null)
            {
                logic.Card = card;
            }
            return logic;
        }
    }
}
