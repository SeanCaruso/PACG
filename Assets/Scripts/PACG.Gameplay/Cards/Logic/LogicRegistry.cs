using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PACG.Gameplay
{
    /// <summary>
    /// RegisterAllLogic must be manually called to register the card logic classes.
    /// </summary>
    public class LogicRegistry
    {
        private readonly Dictionary<string, CardLogicBase> _cardLogicMap = new();

        public void RegisterAllLogic(GameServices gameServices)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract) continue;

                // Check if this is a CardLogicBase subclass
                if (typeof(CardLogicBase).IsAssignableFrom(type))
                {
                    string cardID = null;

                    // Check for new unified LogicFor attribute first
                    var logicAttribute = type.GetCustomAttribute<LogicForAttribute>();
                    if (logicAttribute != null)
                    {
                        cardID = logicAttribute.CardID;
                    }
                    else
                    {
                        // Fall back to legacy attributes for backward compatibility
                        var encounterAttribute = type.GetCustomAttribute<EncounterLogicForAttribute>();
                        if (encounterAttribute != null)
                        {
                            cardID = encounterAttribute.CardID;
                        }
                        else
                        {
                            var playableAttribute = type.GetCustomAttribute<PlayableLogicForAttribute>();
                            if (playableAttribute != null)
                            {
                                cardID = playableAttribute.CardID;
                            }
                        }
                    }

                    if (cardID == null) continue; // No attribute found

                    try
                    {
                        var instance = Activator.CreateInstance(type, gameServices) as CardLogicBase;
                        _cardLogicMap[cardID] = instance;
                    }
                    catch (MissingMethodException ex)
                    {
                        Debug.LogError($"Failed to create {type.Name}: Make sure CreateInstance has the correct constructor signature for CardLogicBase. Exception: {ex.Message}");
                        throw; // Re-throw to make it obvious something is wrong
                    }
                }
            }

            Debug.Log($"Registered {_cardLogicMap.Count} card logics.");
        }

        // Public getter for card logic
        public CardLogicBase GetCardLogic(CardInstance card)
        {
            _cardLogicMap.TryGetValue(card.Data.cardID, out var logic);
            if (logic != null)
            {
                logic.Card = card;
            }

            return logic;
        }
    }
}
