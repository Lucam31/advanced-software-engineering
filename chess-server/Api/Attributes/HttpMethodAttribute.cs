using System;

namespace chess_server.Api.Attributes;

/// <summary>
/// Specifies the HTTP method for an action.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class HttpMethodAttribute : Attribute
{
    /// <summary>
    /// Gets the HTTP method.
    /// </summary>
    public string Method { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpMethodAttribute"/> class.
    /// </summary>
    /// <param name="method">The HTTP method (e.g., "GET", "POST").</param>
    public HttpMethodAttribute(string method)
    {
        Method = method.ToUpper();
    }
}