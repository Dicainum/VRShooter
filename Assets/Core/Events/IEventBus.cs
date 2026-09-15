using System;

namespace VRTraining.Core.Events
{
    public interface IEvent
    {
    }
    //Интерфейс для обмена и управления подписками на ивенты 
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : IEvent;
        void Unsubscribe<T>(Action<T> handler) where T : IEvent;
        void Publish<T>(T payload) where T : IEvent;
        void Clear();
    }
}