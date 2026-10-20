namespace ASP_P42.Models.Rest
{

    public class RestStatus
    {
        public bool   IsOk    { get; set; }
        public int    Code    { get; set; }
        public String Message { get; set; } = null!;


        public static readonly RestStatus Continue           = new() { IsOk = true,  Code = 100, Message = "Continue" };
        public static readonly RestStatus SwitchingProtocols= new() { IsOk = true,  Code = 101, Message = "Switching Protocols" };
        public static readonly RestStatus Processing         = new() { IsOk = true,  Code = 102, Message = "Processing" };
        public static readonly RestStatus EarlyHints        = new() { IsOk = true,  Code = 103, Message = "Early Hints" };


        public static readonly RestStatus Ok                 = new() { IsOk = true,  Code = 200, Message = "OK" };
        public static readonly RestStatus Created            = new() { IsOk = true,  Code = 201, Message = "Created" };
        public static readonly RestStatus Accepted           = new() { IsOk = true,  Code = 202, Message = "Accepted" };
        public static readonly RestStatus NonAuthoritativeInfo = new() { IsOk = true, Code = 203, Message = "Non-Authoritative Information" };
        public static readonly RestStatus NoContent          = new() { IsOk = true,  Code = 204, Message = "No Content" };
        public static readonly RestStatus ResetContent       = new() { IsOk = true,  Code = 205, Message = "Reset Content" };
        public static readonly RestStatus PartialContent     = new() { IsOk = true,  Code = 206, Message = "Partial Content" };
        public static readonly RestStatus MultiStatus        = new() { IsOk = true,  Code = 207, Message = "Multi-Status" };
        public static readonly RestStatus AlreadyReported    = new() { IsOk = true,  Code = 208, Message = "Already Reported" };
        public static readonly RestStatus IMUsed             = new() { IsOk = true,  Code = 226, Message = "IM Used" };

        public static readonly RestStatus MultipleChoices    = new() { IsOk = false, Code = 300, Message = "Multiple Choices" };
        public static readonly RestStatus MovedPermanently   = new() { IsOk = false, Code = 301, Message = "Moved Permanently" };
        public static readonly RestStatus Found              = new() { IsOk = false, Code = 302, Message = "Found" };
        public static readonly RestStatus SeeOther           = new() { IsOk = false, Code = 303, Message = "See Other" };
        public static readonly RestStatus NotModified        = new() { IsOk = false, Code = 304, Message = "Not Modified" };
        public static readonly RestStatus UseProxy           = new() { IsOk = false, Code = 305, Message = "Use Proxy" };
        public static readonly RestStatus TemporaryRedirect  = new() { IsOk = false, Code = 307, Message = "Temporary Redirect" };
        public static readonly RestStatus PermanentRedirect  = new() { IsOk = false, Code = 308, Message = "Permanent Redirect" };


        public static readonly RestStatus BadRequest         = new() { IsOk = false, Code = 400, Message = "Bad Request" };
        public static readonly RestStatus Unauthorized       = new() { IsOk = false, Code = 401, Message = "Unauthorized" };
        public static readonly RestStatus PaymentRequired    = new() { IsOk = false, Code = 402, Message = "Payment Required" };
        public static readonly RestStatus Forbidden          = new() { IsOk = false, Code = 403, Message = "Forbidden" };
        public static readonly RestStatus NotFound           = new() { IsOk = false, Code = 404, Message = "Not Found" };
        public static readonly RestStatus MethodNotAllowed   = new() { IsOk = false, Code = 405, Message = "Method Not Allowed" };
        public static readonly RestStatus NotAcceptable      = new() { IsOk = false, Code = 406, Message = "Not Acceptable" };
        public static readonly RestStatus ProxyAuthRequired  = new() { IsOk = false, Code = 407, Message = "Proxy Authentication Required" };
        public static readonly RestStatus RequestTimeout     = new() { IsOk = false, Code = 408, Message = "Request Timeout" };
        public static readonly RestStatus Conflict           = new() { IsOk = false, Code = 409, Message = "Conflict" };
        public static readonly RestStatus Gone               = new() { IsOk = false, Code = 410, Message = "Gone" };
        public static readonly RestStatus LengthRequired     = new() { IsOk = false, Code = 411, Message = "Length Required" };
        public static readonly RestStatus PreconditionFailed = new() { IsOk = false, Code = 412, Message = "Precondition Failed" };
        public static readonly RestStatus ContentTooLarge    = new() { IsOk = false, Code = 413, Message = "Content Too Large" };
        public static readonly RestStatus UriTooLong         = new() { IsOk = false, Code = 414, Message = "URI Too Long" };
        public static readonly RestStatus UnsupportedMediaType=new() { IsOk = false, Code = 415, Message = "Unsupported Media Type" };
        public static readonly RestStatus RangeNotSatisfiable= new() { IsOk = false, Code = 416, Message = "Range Not Satisfiable" };
        public static readonly RestStatus ExpectationFailed  = new() { IsOk = false, Code = 417, Message = "Expectation Failed" };
        public static readonly RestStatus ImATeapot          = new() { IsOk = false, Code = 418, Message = "I'm a Teapot" };
        public static readonly RestStatus MisdirectedRequest = new() { IsOk = false, Code = 421, Message = "Misdirected Request" };
        public static readonly RestStatus UnprocessableContent= new(){ IsOk = false, Code = 422, Message = "Unprocessable Content" };
        public static readonly RestStatus Locked             = new() { IsOk = false, Code = 423, Message = "Locked" };
        public static readonly RestStatus FailedDependency   = new() { IsOk = false, Code = 424, Message = "Failed Dependency" };
        public static readonly RestStatus TooEarly           = new() { IsOk = false, Code = 425, Message = "Too Early" };
        public static readonly RestStatus UpgradeRequired    = new() { IsOk = false, Code = 426, Message = "Upgrade Required" };
        public static readonly RestStatus PreconditionRequired=new() { IsOk = false, Code = 428, Message = "Precondition Required" };
        public static readonly RestStatus TooManyRequests    = new() { IsOk = false, Code = 429, Message = "Too Many Requests" };
        public static readonly RestStatus HeaderFieldsTooLarge=new() { IsOk = false, Code = 431, Message = "Request Header Fields Too Large" };
        public static readonly RestStatus UnavailableForLegal= new() { IsOk = false, Code = 451, Message = "Unavailable For Legal Reasons" };


        public static readonly RestStatus InternalServerError= new() { IsOk = false, Code = 500, Message = "Internal Server Error" };
        public static readonly RestStatus NotImplemented     = new() { IsOk = false, Code = 501, Message = "Not Implemented" };
        public static readonly RestStatus BadGateway         = new() { IsOk = false, Code = 502, Message = "Bad Gateway" };
        public static readonly RestStatus ServiceUnavailable = new() { IsOk = false, Code = 503, Message = "Service Unavailable" };
        public static readonly RestStatus GatewayTimeout     = new() { IsOk = false, Code = 504, Message = "Gateway Timeout" };
        public static readonly RestStatus HttpVersionNotSupported=new(){IsOk= false, Code = 505, Message = "HTTP Version Not Supported" };
        public static readonly RestStatus VariantAlsoNegotiates=new() { IsOk = false, Code = 506, Message = "Variant Also Negotiates" };
        public static readonly RestStatus InsufficientStorage = new(){ IsOk = false, Code = 507, Message = "Insufficient Storage" };
        public static readonly RestStatus LoopDetected       = new() { IsOk = false, Code = 508, Message = "Loop Detected" };
        public static readonly RestStatus NotExtended        = new() { IsOk = false, Code = 510, Message = "Not Extended" };
        public static readonly RestStatus NetworkAuthRequired = new(){ IsOk = false, Code = 511, Message = "Network Authentication Required" };


        public static readonly RestStatus HeaderRequired      = new() { IsOk = false, Code = 440, Message = "Header Required" };


        public static readonly RestStatus HeaderMalformed     = new() { IsOk = false, Code = 441, Message = "Header Malformed" };

 
        public static readonly RestStatus HeaderValueNotAllowed=new() { IsOk = false, Code = 442, Message = "Header Value Not Allowed" };


        public static readonly RestStatus TokenRequired       = new() { IsOk = false, Code = 450, Message = "Token Required" };


        public static readonly RestStatus TokenExpired        = new() { IsOk = false, Code = 452, Message = "Token Expired" };


        public static readonly RestStatus TokenInvalid        = new() { IsOk = false, Code = 453, Message = "Token Invalid" };


        public static RestStatus Custom(int code, String message, bool isOk = false)
            => new() { IsOk = isOk, Code = code, Message = message };
    }
}
