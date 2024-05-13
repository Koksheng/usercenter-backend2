namespace usercenter.Api.Common
{
    public record BaseResponse<T>(int code, T data, string message = "", string description = "")
    {

    };
}
