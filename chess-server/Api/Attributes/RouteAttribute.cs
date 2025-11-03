namespace chess_server.Api.Attributes;

/// <summary>
/// Specifies the route template for a controller or action.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    private readonly string _path;

    /// <summary>
    /// Initializes a new instance of the <see cref="RouteAttribute"/> class.
    /// </summary>
    /// <param name="path">The route template.</param>
    public RouteAttribute(string path)
    {
        _path = path;
    }

    /// <summary>
    /// Gets the route template.
    /// </summary>
    public string Path => _path;
}