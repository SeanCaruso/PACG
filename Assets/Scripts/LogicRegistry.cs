using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LogicRegistry : MonoBehaviour
{
    private readonly Dictionary<string, IEncounterLogic> encounterLogicMap = new();
    private readonly Dictionary<string, IPlayableLogic> playableLogicMap = new();

    private void Awake()
    {
        ServiceLocator.Register(this);

        RegisterAllLogic();
    }

    private void RegisterAllLogic()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsInterface || type.IsAbstract) continue;
            
            // Check for Encounter logic.
            if (typeof(IEncounterLogic).IsAssignableFrom(type))
            {
                var attribute = type.GetCustomAttribute<EncounterLogicForAttribute>();
                if (attribute != null)
                {
                    var instance = Activator.CreateInstance(type) as IEncounterLogic;
                    encounterLogicMap[attribute.CardID] = instance;
                }
            }

            // Check for Playable logic.
            if (typeof(IPlayableLogic).IsAssignableFrom(type))
            {
                var attribute = type.GetCustomAttribute<PlayableLogicForAttribute>();
                if (attribute != null)
                {
                    var instance = Activator.CreateInstance(type) as IPlayableLogic;
                    playableLogicMap[attribute.CardID] = instance;
                }
            }
        }

        Debug.Log($"Registered {encounterLogicMap.Count} Encounter logics and {playableLogicMap.Count} Playable logics.");
    }

    // Public getters for logic types.
    public IEncounterLogic GetEncounterLogic(string cardID)
    {
        encounterLogicMap.TryGetValue(cardID, out var logic);
        return logic;
    }

    public IPlayableLogic GetPlayableLogic(CardData cardData)
    {
        playableLogicMap.TryGetValue(cardData.cardID, out var logic);
        if (logic != null)
        {
            logic.CardData = cardData;
        }
        return logic;
    }
}
