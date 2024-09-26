namespace ExpenseTracker.Core.DTOs;

public class ResultDto
{
    public ResultDto(object data)
    {
        Data = data;
        Message = "";
    }

    public ResultDto(string message)
    {
        Message = message;
    }

    public ResultDto(object data, string message)
    {
        Message = message;
        Data = data;
    }

    public string Message { get; set; }
    public object Data { get; set; }
}
