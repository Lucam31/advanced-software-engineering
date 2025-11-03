namespace chess_server.Api.Attributes;

/// <summary>
/// Specifies that a parameter should be bound from the request query string.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class FromQueryAttribute : Attribute
{
}