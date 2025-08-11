using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PACG.Gameplay
{
    public class LogicRegistry
    {
        private readonly Dictionary<string, CardLogicBase> _cardLogicMap = new();

        private GameServices _gameServices;

        public void Initialize(GameServices gameServices)
        {
            _gameServices = gameServices;
        }

        private CardLogicBase LoadLogic(string cardID)
        {
            string logicClassName = $"PACG.Gameplay.{cardID}Logic";
            Type logicType = Assembly.GetExecutingAssembly().GetType(logicClassName);

            if (logicType == null)
            {
                Debug.LogError($"[{GetType().Name}] Unable to find logic class {logicClassName}!");
                return null;
            }

            if (!typeof(CardLogicBase).IsAssignableFrom(logicType))
            {
                Debug.LogError($"[{GetType().Name}] Unable to assign {logicClassName} to CardLogicBase!");
                return null;
            }

            return Activator.CreateInstance(logicType, _gameServices) as CardLogicBase;
        }

        // Public getter for card logic
        public CardLogicBase GetCardLogic(string cardID)
        {
            if (!_cardLogicMap.ContainsKey(cardID))
            {
                _cardLogicMap.Add(cardID, LoadLogic(cardID));
            }

            return _cardLogicMap[cardID];
        }
    }
}
