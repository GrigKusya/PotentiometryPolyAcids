namespace KarginScales.Service;

public class LoadResult<T>
{
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public static LoadResult<T> Success(T data) => new LoadResult<T> { Data = data };
    public static LoadResult<T> Failure(string message) => new LoadResult<T> { ErrorMessage = message };
}
