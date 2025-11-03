namespace Shared.Exceptions;

/// <summary>
/// The exception that is thrown when attempting to create a user that already exists.
/// </summary>
public class UserAlreadyExists() : Exception("USER_ALREADY_EXISTS");