using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnyoxGames.Service
{
    public class DefaultServiceManager : IServiceManager
    {
        private readonly Dictionary<Type, IService> services = new();

        public DefaultServiceManager()
        {
            Debug.Log("Created Service Manager");
        }
        
        public bool TryGetService<T>(out T service) where T : class, IService
        {
            if (services.TryGetValue(typeof(T), out IService value))
            {
                service = (T)value;
                return true;
            }
            
            service = null;
            return false;
        }

        public bool TryRegisterService(Type contract, IService service)
        {
            if (!services.ContainsKey(contract) && services.TryAdd(contract, service))
            {
                Debug.Log($"Registered Service: {contract}");
                return true;
            }
            
            Debug.Log($"<color=red>Failed to register service: {contract}</color>");
            return false;
        }

        public bool TryUnregisterService(Type contract)
        {
            return services.ContainsKey(contract) && services.Remove(contract);
        }
    }
}