namespace IMSSampleApplication.Models
{
    public class PingResponse
    {
        public int code { get; set; }
        public string message { get; set; }

        public PingResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }
}
