namespace IMSSampleApplication.Models
{
    public class MultipartMessageResponse
    {
        public string status { get; set; }
        public int code { get; set; }
        public string message { get; set; }

        public MultipartMessageResponse(string status, int code, string message)
        {
            this.status = status;
            this.code = code;
            this.message = message;
        }
    }
}
