using System;
using System.Collections.Generic;

namespace EditorPlus
{
    internal static class StaticUnityEventOrchestrator
    {
        private static readonly Dictionary<string, Action> EventIdTable = new Dictionary<string, Action>();

        public static void SubscribeToEvent(string eventId, Action onEventInvoked)
        {
            if (! EventIdTable.ContainsKey(eventId))
            {
                EventIdTable[eventId] = onEventInvoked;
                return;
            }
            
            EventIdTable[eventId] += onEventInvoked;
        }
        
        public static void UnsubscribeToEvent(string eventId, Action onEventInvoked)
        {
            if (EventIdTable.ContainsKey(eventId))
            {
                EventIdTable[eventId] -= onEventInvoked;
            }
        }

        public static void FireEvent(string eventId)
        {
            if (EventIdTable.TryGetValue(eventId, out var eventAction))
                eventAction.Invoke();
        }
    }
}
