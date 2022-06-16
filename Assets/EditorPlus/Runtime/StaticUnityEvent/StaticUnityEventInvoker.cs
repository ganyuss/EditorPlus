using System;
using UnityEngine;
using UnityEngine.Events;

namespace EditorPlus
{
    [Serializable]
    public class StaticUnityEventInvoker
    {
        [SerializeField]
        private string EventId;

        public void Invoke()
        {
            StaticUnityEventOrchestrator.FireEvent(EventId);
        }
    }
}
