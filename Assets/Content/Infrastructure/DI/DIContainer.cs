using UnityEngine;

public class DIContainer
{
    private static DIContainer _instance;
    public static DIContainer Container => _instance ??= new DIContainer();

    public void RegisterService<TService>(TService implementation) where  TService : IService => 
        Implementation<TService>.ServiceInstance = implementation;

    public TService GetService<TService>() where  TService : IService => 
        Implementation<TService>.ServiceInstance;


    private static class Implementation<TService> where  TService : IService
    {
        public static TService ServiceInstance;
    }
}
