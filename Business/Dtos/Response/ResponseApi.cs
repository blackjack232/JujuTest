using System.Collections.Generic;

public class ResponseApi<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; }
    public T Data { get; set; }

    public ResponseApi(T data, string message = null)
    {
        Succeeded = true;
        Message = message;
        Data = data;
        Errors = new List<string>();
    }

    public ResponseApi(string message)
    {
        Succeeded = false;
        Message = message;
        Errors = new List<string>();
    }

    public ResponseApi(List<string> errors, string message)
    {
        Succeeded = false;
        Message = message;
        Errors = errors;
    }
}