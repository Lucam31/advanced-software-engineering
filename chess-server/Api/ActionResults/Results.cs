namespace chess_server.Api.ActionResults;

public static class Results
{
    public static OkResult Ok() => new();
    public static OkResult Ok(object value) => new(value);
}