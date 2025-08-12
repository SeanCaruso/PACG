using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PACG.Gameplay
{
    public class LogicRegistry
    {
        private readonly Dictionary<string, ILogicBase> _logicMap = new();

        private GameServices _gameServices;

        public void Initialize(GameServices gameServices)
        {
            _gameServices = gameServices;
        }

        private ILogicBase LoadLogic(string cardID)
        {
            string logicClassName = $"PACG.Gameplay.{cardID}Logic";
            Type logicType = Assembly.GetExecutingAssembly().GetType(logicClassName);

            if (logicType == null)
            {
                Debug.LogError($"[{GetType().Name}] Unable to find logic class {logicClassName}!");
                return null;
            }

            if (!typeof(ILogicBase).IsAssignableFrom(logicType))
            {
                Debug.LogError($"[{GetType().Name}] Unable to assign {logicClassName} to ILogicBase!");
                return null;
            }

            return Activator.CreateInstance(logicType, _gameServices) as ILogicBase;
        }

        public CardLogicBase GetCardLogic(string cardID)
        {
            if (!_logicMap.ContainsKey(cardID))
            {
                _logicMap.Add(cardID, LoadLogic(cardID));
            }

            return _logicMap[cardID] as CardLogicBase;
        }

        public CharacterLogicBase GetCharacterLogic(string characterName)
        {
            if (!_logicMap.ContainsKey(characterName))
            {
                _logicMap.Add(characterName, LoadLogic(characterName));
            }

            return _logicMap[characterName] as CharacterLogicBase;
        }
    }
}
