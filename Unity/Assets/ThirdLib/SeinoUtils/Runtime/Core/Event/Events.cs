using System;
using System.Collections.Generic;

namespace SeinoUtils.Runtime.Core.Event
{
    public class Events
    {
        private Dictionary<string, Event> events = new Dictionary<string, Event>();
        
        public Action<T> AddListener<T>(Action<T> listener) where T : class
        {
            var type = typeof(T);
            string key = type.FullName;
            return AddListener<T>(key,listener);
        }
        
        public Action<T> AddListener<T>(string key,Action<T> listener) where T : class
        {
            if(!events.TryGetValue(key,out Event _event))
            {
                _event = new Event<T>();
                events.Add(key,_event);
            } 
            (_event as Event<T>).AddListener(listener);
            return listener;
        }
        
        public void RemoveListener<T>(Action<T> listener) where T : class
        {
            var type = typeof(T);
            string key = type.FullName;
            RemoveListener(key,listener);
        }
        
        public void RemoveListener<T>(string key,Action<T> listener) where T : class
        {
            if(events.TryGetValue(key,out Event _event))
            {
                (_event as Event<T>).RemoveListener(listener);
            }
        }
        
        public void RemoveAllListeners<T>() where T : class
        {
            var type = typeof(T);
            string key = type.FullName;
            RemoveAllListeners(key);
        }
        
        public void RemoveAllListeners(string key)
        {
            if(events.ContainsKey(key))
            {
                events.Remove(key);
            }
        }
        
        public void Call(object message)
        {
            string key = message.GetType().FullName;
            Call(key,message);
        }
        
        public void Call(string key,object message)
        {
            if(events.TryGetValue(key,out Event _event))
            {
                _event.Call(message);
            }
        }
        
        public void Clear()
        {
            events.Clear();
        }
    }   
}