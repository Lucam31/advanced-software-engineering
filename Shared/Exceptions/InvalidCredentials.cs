namespace Shared.Exceptions;

/// <summary>
/// The exception that is thrown when authentication fails due to invalid credentials.
/// </summary>
public class InvalidCredentials()  : Exception("INVALID_CREDENTIALS");