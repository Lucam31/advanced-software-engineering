namespace chess_server.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    private readonly string _path;

    public RouteAttribute(string path)
    {
        _path = path;
    }
    
    public string Path => _path;
}