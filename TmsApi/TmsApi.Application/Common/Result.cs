namespace TmsApi.Application.Common;

public readonly record struct Result<TSuccess, TError>
{
    public bool IsSuccess { get; init; }
    public TSuccess? Success { get; init; }
    public TError? Error { get; init; }

    public static Result<TSuccess, TError> Ok(TSuccess value) => new() { IsSuccess = true, Success = value };
    public static Result<TSuccess, TError> Fail(TError error) => new() { IsSuccess = false, Error = error };
}

public readonly record struct Result<TSuccess>
{
    public bool IsSuccess { get; init; }
    public TSuccess? Success { get; init; }
    public string? Error { get; init; }

    public static Result<TSuccess> Ok(TSuccess value) => new() { IsSuccess = true, Success = value };
    public static Result<TSuccess> Fail(string error) => new() { IsSuccess = false, Error = error };
}

public readonly record struct Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }

    public static Result Ok() => new() { IsSuccess = true };
    public static Result Fail(string error) => new() { IsSuccess = false, Error = error };
}
