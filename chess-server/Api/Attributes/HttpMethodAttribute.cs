using System;

namespace chess_server.Api.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HttpMethodAttribute : Attribute
{
    public string Method { get; }

    public HttpMethodAttribute(string method)
    {
        Method = method.ToUpper();
    }
}