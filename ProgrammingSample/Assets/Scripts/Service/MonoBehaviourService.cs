using UnityEngine;

namespace AnyoxGames.Service
{
    public abstract class MonoBehaviourService : MonoBehaviour, IService
    {
        protected virtual void Awake()
        {
            IServiceManager.Default.TryRegisterService(GetType(), this);
        }

        protected void OnDestroy()
        {
            IServiceManager.Default.TryUnregisterService(GetType());
        }
    }
}