namespace chess_server.Api;

public interface IDiContainer
{
    void Register<TInterface,TImplementation>(Func<TImplementation> implementationFactory) where TImplementation : class, TInterface;
    TService Resolve<TService>();
} 

public class DiContainer : IDiContainer
{
    private readonly Dictionary<Type, Func<object>> _services = new ();
    
    public void Register<T>(Func<T> factory) where T : class
    {
        _services[typeof(T)] = () => factory();
    }
    
    public void Register<TInterface, TImplementation>(Func<TImplementation> factory) where TImplementation : class, TInterface
    {
        _services[typeof(TInterface)] = () => factory();
    }
    
    public TService Resolve<TService>()
    {
        if (_services.TryGetValue(typeof(TService), out var factory))
        {
            return (TService)factory();
        }
        
        throw new Exception($"Service of type {typeof(TService)} not registered.");
    }
}