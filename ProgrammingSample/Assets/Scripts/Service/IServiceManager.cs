using System;

namespace AnyoxGames.Service
{
    public interface IServiceManager
    {
        public static IServiceManager Default { get; set; }

        static IServiceManager()
        {
            Default = new DefaultServiceManager();
        }

        public bool TryGetService<T>(out T service) where T : class, IService;
        
        public bool TryRegisterService<TImplementation>(IService service) where TImplementation : class, IService
            => TryRegisterService(typeof(TImplementation), service);
        public bool TryRegisterService(Type contract, IService service);
        
        public bool TryUnregisterService<TImplementation>(IService service) where TImplementation : class, IService
            => TryUnregisterService(typeof(TImplementation));
        public bool TryUnregisterService(Type contract);
    }
}