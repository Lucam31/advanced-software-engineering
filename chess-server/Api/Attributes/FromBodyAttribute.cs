namespace chess_server.Api.Attributes;

/// <summary>
/// Specifies that a parameter should be bound from the request body.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class FromBodyAttribute : Attribute
{
}