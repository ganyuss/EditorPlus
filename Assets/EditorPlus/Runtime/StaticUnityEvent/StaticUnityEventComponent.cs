using UnityEngine;
using UnityEngine.Events;

namespace EditorPlus
{
    public class StaticUnityEventComponent : MonoBehaviour
    {
        [SerializeField]
        private string EventId;
        [SerializeField]
        private UnityEvent OnEventInvoked;

        private void OnEnable()
        {
            StaticUnityEventOrchestrator.SubscribeToEvent(EventId, OnEventInvoked.Invoke);
        }
        
        private void OnDisable()
        {
            StaticUnityEventOrchestrator.UnsubscribeToEvent(EventId, OnEventInvoked.Invoke);
        }

        public void Invoke()
        {
            StaticUnityEventOrchestrator.FireEvent(EventId);
        }

        public void AddListener(UnityAction call)
        {
            OnEventInvoked.AddListener(call);
        }

        public void RemoveListener(UnityAction call)
        {
            OnEventInvoked.RemoveListener(call);
        }
    }
}
