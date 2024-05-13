namespace usercenter.Api.Common
{
    public class ErrorCode
    {
        public int Code { get; }
        public string Message { get; }
        public string Description { get; }

        private ErrorCode(int code, string message, string description)
        {
            Code = code;
            Message = message;
            Description = description;
        }

        public static readonly ErrorCode SUCCESS = new ErrorCode(0, "ok", "");
        public static readonly ErrorCode PARAMS_ERROR = new ErrorCode(40000, "请求参数错误", "");
        public static readonly ErrorCode NULL_ERROR = new ErrorCode(40001, "请求数据为空", "");
        public static readonly ErrorCode NOT_LOGIN = new ErrorCode(40100, "未登录", "");
        public static readonly ErrorCode NO_AUTH = new ErrorCode(40101, "无权限", "");
        public static readonly ErrorCode STSTEM_ERROR = new ErrorCode(50000, "系统内部异常", "");
        // Add more error codes as needed
    }
}
