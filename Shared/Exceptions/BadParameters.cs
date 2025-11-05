namespace Shared.Exceptions;

/// <summary>
/// Exception that represents invalid or missing parameters provided by the client.
/// </summary>
/// <param name="ex">A short message describing the bad parameter condition.</param>
public class BadParameters(string ex) : Exception(ex);
