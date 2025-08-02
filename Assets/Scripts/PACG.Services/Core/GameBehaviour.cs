using PACG.Services.Game;
using UnityEngine;

namespace PACG.Services.Core
{
    public abstract class GameBehaviour : MonoBehaviour
    {
        protected CardManager Cards;
        protected ContextManager Contexts;
        protected LogicRegistry Logic;

        void Awake()
        {
            ServiceLocator.Register(this);

            OnAwake();
        }

        private void Start()
        {
            Cards = ServiceLocator.Get<CardManager>();
            Contexts = ServiceLocator.Get<ContextManager>();
            Logic = ServiceLocator.Get<LogicRegistry>();
        }

        protected virtual void OnAwake() { }
    }
}
