namespace chess_server.Api.ActionResults;

/// <summary>
/// A factory that provides methods to create various <see cref="IActionResult"/> objects.
/// </summary>
public static class Results
{
    /// <summary>
    /// Creates an <see cref="OkResult"/> that produces an empty HTTP 200 OK response.
    /// </summary>
    /// <returns>The created <see cref="OkResult"/>.</returns>
    public static OkResult Ok() => new();

    /// <summary>
    /// Creates an <see cref="OkResult"/> that produces an HTTP 200 OK response with the specified value.
    /// </summary>
    /// <param name="value">The content to format in the entity body.</param>
    /// <returns>The created <see cref="OkResult"/>.</returns>
    public static OkResult Ok(object value) => new(value);

    /// <summary>
    /// Creates a <see cref="BadRequestResult"/> that produces an HTTP 400 Bad Request response with the specified error message.
    /// </summary>
    /// <param name="message">The error message to return to the client.</param>
    /// <returns>The created <see cref="BadRequestResult"/>.</returns>
    public static BadRequestResult BadRequest(string message) => new(message);
}