namespace Shared.Exceptions;

/// <summary>
/// The exception that is thrown when a requested user is not found.
/// </summary>
public class UserNotFound() : Exception("USER_NOT_FOUND");