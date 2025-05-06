using System;

namespace SeinoUtils.Runtime.Core.Event
{
    abstract class Event 
    {
        public abstract void Call(object message);
    }
    
    class Event<T> : Event where T : class
    {
        public Action<T> handler;
        
        public void AddListener(Action<T> listener)
        {
            handler += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            handler -= listener;
        }

        public override void Call(object message)
        {
            if(handler != null)
                handler.Invoke(message as T);
        }
    }
}