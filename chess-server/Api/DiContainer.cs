namespace chess_server.Api;

using Shared.Logger;

/// <summary>
/// Defines the interface for a dependency injection container.
/// </summary>
public interface IDiContainer
{
    /// <summary>
    /// Registers a service with its implementation.
    /// </summary>
    /// <typeparam name="TInterface">The interface of the service.</typeparam>
    /// <typeparam name="TImplementation">The implementation of the service.</typeparam>
    void Register<TInterface, TImplementation>()
        where TImplementation : class, TInterface;

    /// <summary>
    /// Resolves a service to get an instance of it.
    /// </summary>
    /// <typeparam name="TService">The type of the service to resolve.</typeparam>
    /// <returns>An instance of the service.</returns>
    TService Resolve<TService>();
}

/// <summary>
/// A simple dependency injection container.
/// </summary>
public class DiContainer : IDiContainer
{
    private readonly Dictionary<Type, Func<object>> _services = new();

    /// <summary>
    /// Registers a service with a factory function.
    /// </summary>
    /// <typeparam name="T">The type of the service to register.</typeparam>
    public void Register<T>() where T : class
    {
        _services[typeof(T)] = CreateInstance<T>;
        GameLogger.Debug($"Service registered: {typeof(T).Name}");
    }

    /// <summary>
    /// For certain objects it may be necessary to pass a factory function
    /// </summary>
    /// <param name="factory">Function to create an instance</param>
    public void Register<T>(Func<T> factory) where T : class
    {
        _services[typeof(T)] = () => factory();
    }

    /// <inheritdoc/>
    public void Register<TInterface, TImplementation>()
        where TImplementation : class, TInterface
    {
        _services[typeof(TInterface)] = CreateInstance<TImplementation>;
        GameLogger.Debug($"Service registered: {typeof(TInterface).Name} -> {typeof(TImplementation).Name}");
    }

    /// <inheritdoc/>
    public TService Resolve<TService>()
    {
        if (_services.TryGetValue(typeof(TService), out var factory))
        {
            var instance = (TService)factory();
            GameLogger.Debug($"Service resolved: {typeof(TService).Name}");
            return instance;
        }

        var msg = $"Service of type {typeof(TService)} not registered.";
        GameLogger.Error(msg);
        throw new Exception(msg);
    }
    
    /// <summary>
    /// Searches for the constructor of the object and creates an instance
    /// </summary>
    private T CreateInstance<T>() where T : class
    {
        var type = typeof(T);
        
        var constructors = type.GetConstructors();
        if (constructors.Length == 0)
        {
            throw new Exception($"No public constructor found for {type.Name}");
        }

        var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
        var parameters = constructor.GetParameters();

        // Resolve dependencies
        var parameterInstances = parameters.Select(p =>
        {
            try
            {
                var method = typeof(DiContainer).GetMethod("Resolve")!
                    .MakeGenericMethod(p.ParameterType);
                return method.Invoke(this, null);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Cannot resolve dependency {p.ParameterType.Name} for {type.Name}", ex);
            }
        }).ToArray();

        return (T)Activator.CreateInstance(type, parameterInstances)!;
    }
}