using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Core.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();
        private readonly Stack<List<Delegate>> _bufferPool = new Stack<List<Delegate>>();

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var handlers))
            {
                handlers = new List<Delegate>();
                _handlers.Add(type, handlers);
            }

            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null || !_handlers.TryGetValue(typeof(T), out var handlers))
            {
                return;
            }

            handlers.Remove(handler);
        }

        public void Publish<T>(T payload) where T : IEvent
        {
            if (!_handlers.TryGetValue(typeof(T), out var handlers) || handlers.Count == 0)
            {
                return;
            }

            var buffer = RentBuffer();
            buffer.AddRange(handlers);

            try
            {
                for (var i = 0; i < buffer.Count; i++)
                {
                    try
                    {
                        ((Action<T>)buffer[i]).Invoke(payload);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }
            finally
            {
                ReturnBuffer(buffer);
            }
        }

        public void Clear()
        {
            _handlers.Clear();
        }

        private List<Delegate> RentBuffer()
        {
            return _bufferPool.Count > 0 ? _bufferPool.Pop() : new List<Delegate>();
        }

        private void ReturnBuffer(List<Delegate> buffer)
        {
            buffer.Clear();
            _bufferPool.Push(buffer);
        }
    }
}