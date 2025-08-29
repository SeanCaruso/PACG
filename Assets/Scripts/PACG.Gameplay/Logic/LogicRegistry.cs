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

        public T GetLogic<T>(string key) where T : class, ILogicBase
        {
            if (!_logicMap.ContainsKey(key))
            {
                _logicMap.Add(key, LoadLogic(key));
            }

            return _logicMap[key] as T;
        }
    }
}
