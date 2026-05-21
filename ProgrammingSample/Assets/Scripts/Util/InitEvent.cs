using System;
using UnityEngine;

namespace AnyoxGames.Util
{
    public class InitEvent<T>
    {
        private event Action<T> Event;
        private T context;

        public void Subscribe(Action<T> callback)
        {
            if (context == null)
            {
                Event += callback;
            }
            else
            {
                callback?.Invoke(context);
            }
        }

        public void Invoke(T ctx)
        {
            if (context == null)
            {
                throw new NullReferenceException($"Context is null in InitEvent<{typeof(T)}>");
            }

            context = ctx;
            Debug.Log($"[InitEvent] Context of type {typeof(T)} defined, invoking event");
            Event?.Invoke(this.context);
            Event = null;
        }
    }
}