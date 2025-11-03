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
    /// <param name="implementationFactory">A factory function to create an instance of the implementation.</param>
    void Register<TInterface, TImplementation>(Func<TImplementation> implementationFactory)
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
    /// <param name="factory">A factory function to create an instance of the service.</param>
    public void Register<T>(Func<T> factory) where T : class
    {
        _services[typeof(T)] = () => factory();
        GameLogger.Debug($"Service registered: {typeof(T).Name}");
    }

    /// <inheritdoc/>
    public void Register<TInterface, TImplementation>(Func<TImplementation> factory)
        where TImplementation : class, TInterface
    {
        _services[typeof(TInterface)] = () => factory();
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
}