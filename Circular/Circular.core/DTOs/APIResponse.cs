namespace Circular.Core.DTOs
{
    public class APIResponse
    {
        public APIResponse()
        {
            StatusCode = 2000;
        }
        public string Status
        {
            get
            {
                //return StatusCode.ToString();
                return Enum.GetName(typeof(APIResponseCode), StatusCode);
            }
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public Object Data { get; set; }
    }

    public enum APIResponseCode
    {
        Success = 2000,
        Record_Found = 2001,
        No_Record_Found = 2002,
        Record_Already_Exists = 2003,
        Password_Does_Not_Match = 2004,
        OTP_Does_Not_Match = 2005,
        Password_Rules_Validation_Failed = 2006,
        Incomplete_Details = 2007,
        Request_In_Pending = 2008,
        Not_Allowed = 2009,
        Invalid_Request = 2010,
        Different_Currency = 2997,
        Existing_Owner = 2998,
        Failure = 2999


    }
}
